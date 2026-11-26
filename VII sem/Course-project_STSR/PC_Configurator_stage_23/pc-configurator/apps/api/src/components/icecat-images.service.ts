import { Injectable, Logger, ServiceUnavailableException } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Component } from './entities/component.entity';

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
    if (!username || !password) throw new ServiceUnavailableException('Укажите ICECAT_USERNAME и ICECAT_PASSWORD в .env и перезапустите приложение.');

    const items = await this.components.find();
    let updated = 0; let skipped = 0; let failed = 0;
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
        const response = await fetch(url, { headers: { Authorization: `Basic ${token}`, Accept: 'application/xml' }, signal: AbortSignal.timeout(20000) });
        if (!response.ok) {
          if (response.status === 404) { skipped += 1; continue; }
          throw new Error(`Icecat вернул HTTP ${response.status}`);
        }
        const xml = await response.text();
        const images = this.allImages(xml);
        if (!images.length) { skipped += 1; continue; }
        item.imageUrl = images[0];
        item.imageUrls = JSON.stringify(images.slice(0, 12));
        item.imageSource = 'Open Icecat';
        await this.components.save(item);
        updated += 1;
      } catch (error) {
        failed += 1;
        this.logger.warn(`Icecat: не удалось получить фото для ${item.manufacturer} ${item.model}: ${this.errorText(error)}`);
      }
      await new Promise<void>((resolve) => setTimeout(resolve, 250));
    }
    return { source: 'Open Icecat', updated, skipped, failed };
  }

  private allImages(xml: string): string[] {
    const tags = xml.match(/<(?:Image|ProductPicture)\b[^>]*\/?\s*>/gi) ?? [];
    const ranked = tags.map((tag) => ({
      main: /\bIsMain=["']Y["']/i.test(tag),
      url: this.attribute(tag, 'HighPic') ?? this.attribute(tag, 'Pic') ?? this.attribute(tag, 'LowPic'),
    })).filter((entry): entry is { main: boolean; url: string } => Boolean(entry.url && /^https:\/\//i.test(entry.url)));
    const productTag = xml.match(/<Product\b[^>]*>/i)?.[0];
    if (productTag) {
      const url = this.attribute(productTag, 'HighPic') ?? this.attribute(productTag, 'Pic') ?? this.attribute(productTag, 'LowPic');
      if (url && /^https:\/\//i.test(url)) ranked.unshift({ main: true, url });
    }
    const unique = [...new Set([...ranked.filter((entry) => entry.main), ...ranked.filter((entry) => !entry.main)].map((entry) => entry.url))];
    return unique;
  }

  private attribute(tag: string, name: string): string | null {
    const match = tag.match(new RegExp(`\b${name}=["']([^"']+)["']`, 'i'));
    return match?.[1]?.replace(/&amp;/g, '&') ?? null;
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

  private errorText(error: unknown) { return error instanceof Error ? error.message : String(error); }
}
