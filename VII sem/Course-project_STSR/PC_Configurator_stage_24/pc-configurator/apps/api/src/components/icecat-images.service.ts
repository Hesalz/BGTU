import { Injectable, Logger, ServiceUnavailableException } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Component } from './entities/component.entity';

type IcecatPicture = { main: boolean; order: number; full: string | null; preview: string | null; type: string };

@Injectable()
export class IcecatImagesService {
  private readonly logger = new Logger(IcecatImagesService.name);

  constructor(
    @InjectRepository(Component) private readonly components: Repository<Component>,
    private readonly config: ConfigService,
  ) {}

  async enrichImages() {
    const username = this.config.get<string>('ICECAT_USERNAME');
    const password = this.config.get<string>('ICECAT_PASSWORD');
    if (!username || !password) {
      throw new ServiceUnavailableException('Укажите ICECAT_USERNAME и ICECAT_PASSWORD в .env и перезапустите приложение.');
    }

    const items = await this.components.find();
    let updated = 0;
    let skipped = 0;
    let failed = 0;
    let imagesFound = 0;

    for (const item of items) {
      const partNumber = this.partNumber(item);
      if (!partNumber) { skipped += 1; continue; }

      try {
        const url = new URL('https://data.icecat.biz/xml_s3/xml_server3.cgi');
        url.searchParams.set('prod_id', partNumber);
        url.searchParams.set('vendor', item.manufacturer);
        url.searchParams.set('lang', 'EN');
        url.searchParams.set('output', 'productxml');

        const token = Buffer.from(`${username}:${password}`).toString('base64');
        const response = await fetch(url, {
          headers: { Authorization: `Basic ${token}`, Accept: 'application/xml' },
          signal: AbortSignal.timeout(20000),
        });
        if (response.status === 404) { skipped += 1; continue; }
        if (!response.ok) throw new Error(`Icecat вернул HTTP ${response.status}`);

        const xml = await response.text();
        const pictures = this.readPictures(xml);
        if (!pictures.length) { skipped += 1; continue; }

        const fullUrls = this.mergeUrls(
          pictures.map((picture) => picture.full).filter((value): value is string => Boolean(value)),
          this.parseUrls(item.imageUrls),
          item.imageUrl ? [item.imageUrl] : [],
        ).slice(0, 12);
        const previewUrls = this.mergeUrls(
          pictures.map((picture) => picture.preview ?? picture.full).filter((value): value is string => Boolean(value)),
          this.parseUrls(item.imagePreviewUrls),
          item.imagePreviewUrl ? [item.imagePreviewUrl] : [],
          this.parseUrls(item.imageUrls),
          item.imageUrl ? [item.imageUrl] : [],
        ).slice(0, 12);

        if (!fullUrls.length) { skipped += 1; continue; }

        // Icecat is the preferred image source. Keep its main picture first and
        // retain already imported images if the current response has fewer views.
        item.imageUrl = fullUrls[0];
        item.imageUrls = JSON.stringify(fullUrls);
        item.imagePreviewUrl = previewUrls[0] ?? fullUrls[0];
        item.imagePreviewUrls = JSON.stringify(previewUrls.length ? previewUrls : fullUrls);
        item.imageSource = 'Open Icecat';
        await this.components.save(item);
        updated += 1;
        imagesFound += fullUrls.length;
      } catch (error) {
        failed += 1;
        this.logger.warn(`Icecat: не удалось получить фото для ${item.manufacturer} ${item.model}: ${this.errorText(error)}`);
      }
      await this.pause(250);
    }

    return { source: 'Open Icecat', updated, imagesFound, skipped, failed };
  }

  /** Reads the actual ProductGallery/ProductPicture structure documented by Icecat. */
  private readPictures(xml: string): IcecatPicture[] {
    const pictures: IcecatPicture[] = [];
    const productPictures = xml.match(/<ProductPicture\b[^>]*>/gi) ?? [];

    for (const tag of productPictures) {
      const type = this.attribute(tag, 'Type') ?? '';
      // Feature logos and the fallback brand logo are not product views.
      if (/featurelogo/i.test(type)) continue;
      const orderValue = Number(this.attribute(tag, 'No') ?? 9999);
      if (orderValue === 999) continue;
      const full = this.imageUrl(
        this.attribute(tag, 'Pic') ??
        this.attribute(tag, 'Original') ??
        this.attribute(tag, 'HighPic') ??
        this.attribute(tag, 'Pic500x500') ??
        this.attribute(tag, 'LowPic'),
      );
      const preview = this.imageUrl(
        this.attribute(tag, 'Pic500x500') ??
        this.attribute(tag, 'LowPic') ??
        this.attribute(tag, 'ThumbPic') ??
        this.attribute(tag, 'Pic') ??
        this.attribute(tag, 'HighPic'),
      );
      if (!full && !preview) continue;
      pictures.push({
        main: this.attribute(tag, 'IsMain')?.toUpperCase() === 'Y',
        order: Number.isFinite(orderValue) ? orderValue : 9999,
        full: full ?? preview,
        preview: preview ?? full,
        type,
      });
    }

    // Product-level picture attributes are a safe fallback for XML variants
    // that do not include ProductGallery.
    if (!pictures.length) {
      const productTag = xml.match(/<Product\b[^>]*>/i)?.[0];
      if (productTag) {
        const full = this.imageUrl(this.attribute(productTag, 'HighPic') ?? this.attribute(productTag, 'Pic'));
        const preview = this.imageUrl(this.attribute(productTag, 'Pic500x500') ?? this.attribute(productTag, 'LowPic') ?? full);
        if (full || preview) pictures.push({ main: true, order: 1, full: full ?? preview, preview: preview ?? full, type: 'Product' });
      }
    }

    pictures.sort((a, b) => Number(b.main) - Number(a.main) || a.order - b.order);
    const seen = new Set<string>();
    return pictures.filter((picture) => {
      const key = picture.full ?? picture.preview ?? '';
      if (!key || seen.has(key)) return false;
      seen.add(key);
      return true;
    });
  }

  private attribute(tag: string, name: string): string | null {
    // Double escaping is required: this RegExp is built from a JS string.
    const match = tag.match(new RegExp(`\\b${name}\\s*=\\s*["']([^"']*)["']`, 'i'));
    return match?.[1] ? this.decodeXml(match[1]) : null;
  }

  private imageUrl(value: string | null): string | null {
    if (!value) return null;
    const decoded = this.decodeXml(value.trim());
    const normalized = decoded.startsWith('//') ? `https:${decoded}` : decoded;
    return /^https?:\/\//i.test(normalized) ? normalized : null;
  }

  private decodeXml(value: string): string {
    return value
      .replace(/&amp;/g, '&')
      .replace(/&quot;/g, '"')
      .replace(/&apos;/g, "'")
      .replace(/&lt;/g, '<')
      .replace(/&gt;/g, '>');
  }

  private parseUrls(value: string | null): string[] {
    if (!value) return [];
    try {
      const parsed: unknown = JSON.parse(value);
      return Array.isArray(parsed) ? parsed.filter((url): url is string => typeof url === 'string' && /^https?:\/\//i.test(url)) : [];
    } catch { return []; }
  }

  private mergeUrls(...groups: (string | null)[][]): string[] {
    return [...new Set(groups.flat().filter((url): url is string => Boolean(url && /^https?:\/\//i.test(url))))];
  }

  private partNumber(item: Component): string | null {
    if (item.manufacturerPartNumber?.trim()) return item.manufacturerPartNumber.trim();
    try {
      const parsed = JSON.parse(item.specifications ?? '{}') as { metadata?: { part_numbers?: unknown }; part_numbers?: unknown };
      const value = parsed.metadata?.part_numbers ?? parsed.part_numbers;
      if (Array.isArray(value)) return value.find((part): part is string => typeof part === 'string' && Boolean(part.trim()))?.trim() ?? null;
      if (typeof value === 'string' && value.trim()) return value.trim();
    } catch { /* Manually entered specification text is not an OpenDB object. */ }
    return null;
  }

  private pause(ms: number) { return new Promise<void>((resolve) => setTimeout(resolve, ms)); }
  private errorText(error: unknown) { return error instanceof Error ? error.message : String(error); }
}
