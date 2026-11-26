import { Injectable, Logger } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Component } from './entities/component.entity';

type CatalogProduct = Record<string, unknown>;
type PriceOffer = { shop: string; price: number; url: string | null; source?: string };

@Injectable()
export class OnlinerCatalogService {
  private readonly logger = new Logger(OnlinerCatalogService.name);
  private readonly searchRoot = 'https://www.onliner.by/sdapi/catalog.api/search/products';

  constructor(@InjectRepository(Component) private readonly components: Repository<Component>) {}

  async enrichCatalog() {
    const components = await this.components.find();
    let updated = 0;
    let updatedPrices = 0;
    let updatedImages = 0;
    let updatedOffers = 0;
    let clearedZeroPrices = 0;
    let skipped = 0;
    let failed = 0;

    for (const component of components) {
      let hadZeroPrice = false;
      try {
        hadZeroPrice = component.price != null && Number(component.price) <= 0;
        if (hadZeroPrice) {
          component.price = null;
          component.priceSource = null;
          component.priceUrl = null;
          component.priceUpdatedAt = null;
          clearedZeroPrices += 1;
        }

        const product = await this.findProduct(component);
        if (!product) {
          if (hadZeroPrice) await this.components.save(component);
          skipped += 1;
          continue;
        }

        const details = await this.details(product);
        const onlinerOffers = this.extractOffers(details)
          .concat(this.extractOffers(product))
          .filter((offer, index, all) => all.findIndex((candidate) =>
            candidate.shop === offer.shop && candidate.price === offer.price && candidate.url === offer.url) === index)
          .sort((a, b) => a.price - b.price)
          .slice(0, 20);

        const price = onlinerOffers.length
          ? Math.min(...onlinerOffers.map((offer) => offer.price))
          : this.price(details) ?? this.price(product);
        const onlinerImages = [...new Set([...this.images(details), ...this.images(product)])].slice(0, 12);

        if (price == null && !onlinerImages.length && !onlinerOffers.length) {
          if (component.price == null) await this.components.save(component);
          skipped += 1;
          continue;
        }

        if (price != null) {
          component.price = price;
          component.priceSource = 'Каталог Onliner';
          component.priceUrl = this.htmlUrl(product);
          component.priceUpdatedAt = new Date();
          updatedPrices += 1;
        }

        if (onlinerOffers.length) {
          const existingOffers = this.parseOffers(component.priceOffers)
            .filter((offer) => offer.source !== 'Каталог Onliner');
          component.priceOffers = JSON.stringify([...existingOffers, ...onlinerOffers.map((offer) => ({ ...offer, source: 'Каталог Onliner' }))].slice(0, 60));
          updatedOffers += onlinerOffers.length;
        }

        if (onlinerImages.length) {
          // Keep Icecat as the preferred image source; Onliner images are a
          // fallback/addition, never a replacement for an existing Icecat gallery.
          const existingFull = this.mergeUrls(this.parseUrls(component.imageUrls), component.imageUrl ? [component.imageUrl] : []);
          const existingPreview = this.mergeUrls(this.parseUrls(component.imagePreviewUrls), component.imagePreviewUrl ? [component.imagePreviewUrl] : []);
          const preferExisting = component.imageSource === 'Open Icecat';
          const full = this.mergeUrls(preferExisting ? existingFull : [], onlinerImages, preferExisting ? [] : existingFull).slice(0, 12);
          const previews = this.mergeUrls(
            preferExisting ? existingPreview : [],
            onlinerImages,
            preferExisting ? [] : existingPreview,
          ).slice(0, 12);
          component.imageUrl = full[0] ?? component.imageUrl;
          component.imageUrls = JSON.stringify(full);
          component.imagePreviewUrl = previews[0] ?? component.imagePreviewUrl ?? full[0];
          component.imagePreviewUrls = JSON.stringify(previews.length ? previews : full);
          if (!preferExisting) component.imageSource = 'Каталог Onliner';
          updatedImages += 1;
        }

        await this.components.save(component);
        updated += 1;
      } catch (error) {
        if (hadZeroPrice) {
          await this.components.save(component).catch((saveError: unknown) =>
            this.logger.warn(`Не удалось очистить старую нулевую цену: ${this.errorText(saveError)}`));
        }
        failed += 1;
        this.logger.warn(`Не удалось обновить ${component.manufacturer} ${component.model}: ${this.errorText(error)}`);
      }
      await this.pause(350);
    }

    return { source: 'Каталог Onliner', updated, updatedPrices, updatedImages, updatedOffers, clearedZeroPrices, skipped, failed };
  }

  private async findProduct(component: Component): Promise<CatalogProduct | null> {
    const queries = [...new Set([
      [component.manufacturerPartNumber, component.manufacturer, component.model].filter(Boolean).join(' '),
      [component.manufacturer, component.model].filter(Boolean).join(' '),
      component.model,
      component.manufacturerPartNumber ?? '',
    ].map((query) => query.trim()).filter(Boolean))];
    const candidates = new Map<string, CatalogProduct>();

    for (const query of queries) {
      const result = await this.fetchJson<unknown>(`${this.searchRoot}?query=${encodeURIComponent(query)}`);
      for (const product of this.products(result)) {
        const key = typeof product.url === 'string'
          ? product.url
          : String(product.key ?? product.full_name ?? product.name ?? candidates.size);
        candidates.set(key, product);
      }
      const ranked = [...candidates.values()]
        .map((product) => ({ product, score: this.score(product, component) }))
        .sort((a, b) => b.score - a.score);
      if (ranked[0]?.score >= 0.96) return ranked[0].product;
    }

    const best = [...candidates.values()]
      .map((product) => ({ product, score: this.score(product, component) }))
      .sort((a, b) => b.score - a.score)[0];
    return best && best.score >= 0.82 ? best.product : null;
  }

  private score(product: CatalogProduct, component: Component): number {
    const text = this.clean([
      product.name, product.full_name, product.key, product.html_url,
      product.brand, product.manufacturer, product.producer,
    ].filter((value): value is string => typeof value === 'string').join(' '));
    if (!text) return 0;

    const part = this.clean(component.manufacturerPartNumber ?? '');
    if (part.length >= 5 && text.replace(/ /g, '').includes(part.replace(/ /g, ''))) return 1;

    const modelTokens = this.clean(component.model).split(' ').filter((word) => word.length > 1);
    if (!modelTokens.length) return 0;
    const matched = modelTokens.filter((word) => text.includes(word)).length;
    const modelScore = matched / modelTokens.length;
    const manufacturer = this.clean(component.manufacturer);
    const brandTokens = manufacturer.split(' ').filter((word) => word.length > 1);
    const brandMatch = !brandTokens.length || brandTokens.some((word) => text.includes(word));

    // Without a manufacturer part number, require a strong model-token match.
    // This deliberately prefers a skipped item over a price from a neighbouring SKU.
    if (modelScore === 1 && brandMatch) return 0.96;
    if (modelScore >= 0.9 && brandMatch) return 0.86;
    return 0;
  }

  private async details(product: CatalogProduct): Promise<CatalogProduct> {
    const url = typeof product.url === 'string' ? product.url : null;
    if (!url || !/^https:\/\//i.test(url)) return product;
    try { return await this.fetchJson<CatalogProduct>(url); } catch { return product; }
  }

  private products(value: unknown): CatalogProduct[] {
    if (Array.isArray(value)) return value.filter((item): item is CatalogProduct => Boolean(item) && typeof item === 'object');
    if (!value || typeof value !== 'object') return [];
    const record = value as Record<string, unknown>;
    for (const key of ['products', 'items', 'results']) {
      if (Array.isArray(record[key])) return (record[key] as unknown[]).filter((item): item is CatalogProduct => Boolean(item) && typeof item === 'object');
    }
    return [];
  }

  private price(product: CatalogProduct): number | null {
    const prices = product.prices && typeof product.prices === 'object'
      ? product.prices as Record<string, unknown>
      : {};
    const raw = [
      product.price, product.price_min, product.min_price,
      prices.price_min, prices.min, prices.amount, prices.price,
    ].find((value) => value != null);
    const candidate = raw && typeof raw === 'object'
      ? (raw as Record<string, unknown>).amount ?? (raw as Record<string, unknown>).value
      : raw;
    const value = typeof candidate === 'number'
      ? candidate
      : Number(String(candidate ?? '').replace(/\s/g, '').replace(',', '.'));
    return Number.isFinite(value) && value > 0 ? value : null;
  }

  private images(product: CatalogProduct): string[] {
    const result: string[] = [];
    const collect = (value: unknown) => {
      if (typeof value === 'string' && /^https?:\/\//i.test(value)) result.push(value);
      else if (Array.isArray(value)) value.forEach(collect);
      else if (value && typeof value === 'object') {
        const record = value as Record<string, unknown>;
        for (const key of ['url', 'src', 'high', 'highpic', 'header', 'image', 'image_url', 'imageUrl']) {
          if (record[key] != null) collect(record[key]);
        }
      }
    };
    collect(product.images);
    collect(product.image_url);
    collect(product.image);
    return [...new Set(result)];
  }

  private extractOffers(product: CatalogProduct): PriceOffer[] {
    const found: PriceOffer[] = [];
    const roots = [
      product.retailer_listings, product.offers,
      (product.prices && typeof product.prices === 'object'
        ? (product.prices as Record<string, unknown>).offers
        : undefined),
    ];

    const visit = (value: unknown, inheritedShop = 'Onliner') => {
      if (Array.isArray(value)) {
        value.forEach((entry) => visit(entry, inheritedShop));
        return;
      }
      if (!value || typeof value !== 'object') return;
      const record = value as Record<string, unknown>;
      const shopValue = record.shop_name ?? record.retailer ?? record.seller ?? (typeof record.shop === 'string' ? record.shop : undefined) ??
        (record.shop && typeof record.shop === 'object' ? (record.shop as Record<string, unknown>).name : undefined) ??
        (typeof record.name === 'string' ? record.name : undefined);
      const raw = record.price ?? record.amount ?? record.price_value;
      const candidate = raw && typeof raw === 'object'
        ? (raw as Record<string, unknown>).amount ?? (raw as Record<string, unknown>).value
        : raw;
      const price = typeof candidate === 'number'
        ? candidate
        : Number(String(candidate ?? '').replace(/\s/g, '').replace(',', '.'));
      const urlValue = record.html_url ?? record.url ?? record.link ??
        (record.shop && typeof record.shop === 'object' ? (record.shop as Record<string, unknown>).url : undefined);

      if (Number.isFinite(price) && price > 0) {
        found.push({
          shop: typeof shopValue === 'string' ? shopValue : inheritedShop,
          price,
          url: typeof urlValue === 'string' && /^https?:\/\//i.test(urlValue) ? urlValue : null,
        });
      }

      // Recurse only through known offer containers, not arbitrary nested price
      // metadata, to avoid turning min/max summaries into fake shop offers.
      for (const key of ['retailer_listings', 'offers', 'shops', 'items']) {
        if (record[key] && typeof record[key] === 'object') visit(record[key], key);
      }
    };

    roots.forEach((root) => visit(root));
    const unique = new Map<string, PriceOffer>();
    found.forEach((offer) => unique.set(`${offer.shop}|${offer.price}|${offer.url ?? ''}`, offer));
    return [...unique.values()].sort((a, b) => a.price - b.price).slice(0, 20);
  }

  private parseOffers(value: string | null): PriceOffer[] {
    if (!value) return [];
    try {
      const parsed: unknown = JSON.parse(value);
      if (!Array.isArray(parsed)) return [];
      return parsed.filter((offer): offer is PriceOffer =>
        Boolean(offer) && typeof offer.shop === 'string' && Number.isFinite(Number(offer.price)));
    } catch { return []; }
  }

  private parseUrls(value: string | null): string[] {
    if (!value) return [];
    try {
      const parsed: unknown = JSON.parse(value);
      return Array.isArray(parsed)
        ? parsed.filter((url): url is string => typeof url === 'string' && /^https?:\/\//i.test(url))
        : [];
    } catch { return []; }
  }

  private mergeUrls(...groups: string[][]): string[] {
    return [...new Set(groups.flat())];
  }

  private htmlUrl(product: CatalogProduct): string | null {
    const value = product.html_url ?? product.url;
    return typeof value === 'string' && /^https?:\/\//i.test(value) ? value : null;
  }

  private async fetchJson<T>(url: string): Promise<T> {
    const response = await fetch(url, {
      headers: { Accept: 'application/json', 'User-Agent': 'PC-Configurator-Educational-Project' },
      signal: AbortSignal.timeout(20000),
    });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return await response.json() as T;
  }

  private clean(value: string) { return value.toLocaleLowerCase().replace(/[^a-zа-я0-9]+/gi, ' ').trim(); }
  private pause(ms: number) { return new Promise<void>((resolve) => setTimeout(resolve, ms)); }
  private errorText(error: unknown) { return error instanceof Error ? error.message : String(error); }
}
