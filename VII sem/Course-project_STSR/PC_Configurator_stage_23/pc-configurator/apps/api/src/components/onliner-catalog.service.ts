import { Injectable, Logger } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Component } from './entities/component.entity';

type CatalogProduct = Record<string, unknown>;

@Injectable()
export class OnlinerCatalogService {
  private readonly logger = new Logger(OnlinerCatalogService.name);
  private readonly searchRoot = 'https://www.onliner.by/sdapi/catalog.api/search/products';

  constructor(@InjectRepository(Component) private readonly components: Repository<Component>) {}

  async enrichCatalog() {
    const components = await this.components.find();
    let updated = 0; let updatedPrices = 0; let updatedImages = 0; let clearedZeroPrices = 0; let skipped = 0; let failed = 0;
    for (const component of components) {
      let hadZeroPrice = false;
      try {
        // Remove zero values left by earlier versions of this importer.
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
        const offers = [...this.priceOffers(details), ...this.priceOffers(product)]
          .filter((offer, index, all) => all.findIndex((candidate) =>
            candidate.shop === offer.shop && candidate.price === offer.price && candidate.url === offer.url) === index)
          .sort((a, b) => a.price - b.price)
          .slice(0, 20);
        const price = offers.length
          ? Math.min(...offers.map((offer) => offer.price))
          : this.price(details) ?? this.price(product);
        const imageUrls = [...new Set([...this.images(details), ...this.images(product)])].slice(0, 12);
        const imageUrl = imageUrls[0] ?? null;
        if (price == null && !imageUrl && !offers.length) {
          if (component.price == null) await this.components.save(component);
          skipped += 1;
          continue;
        }
        if (price != null) { component.price = price; component.priceSource = 'Каталог Onliner'; component.priceUrl = this.htmlUrl(product); component.priceUpdatedAt = new Date(); updatedPrices += 1; }
        if (imageUrls.length) {
          component.imageUrl = imageUrl;
          component.imageUrls = JSON.stringify(imageUrls);
          component.imageSource = 'Каталог Onliner';
          updatedImages += 1;
        }
        if (offers.length) component.priceOffers = JSON.stringify(offers);
        await this.components.save(component); updated += 1;
      } catch (error) {
        if (hadZeroPrice) await this.components.save(component).catch((saveError: unknown) => this.logger.warn(`Не удалось очистить старую нулевую цену: ${this.errorText(saveError)}`));
        failed += 1;
        this.logger.warn(`Не удалось обновить ${component.manufacturer} ${component.model}: ${this.errorText(error)}`);
      }
      // Respect the catalogue: one request sequence at a time, with a short pause.
      await this.pause(350);
    }
    return { source: 'Каталог Onliner', updated, updatedPrices, updatedImages, clearedZeroPrices, skipped, failed };
  }

  private async findProduct(component: Component): Promise<CatalogProduct | null> {
    const queries = [...new Set([
      [component.manufacturerPartNumber, component.manufacturer, component.model].filter(Boolean).join(' '),
      [component.manufacturer, component.model].filter(Boolean).join(' '),
      component.model,
      component.manufacturerPartNumber ?? '',
    ].map((query) => query.trim()).filter(Boolean))];
    const target = this.clean([component.manufacturer, component.model].filter(Boolean).join(' '));
    const candidates = new Map<string, CatalogProduct>();
    for (const query of queries) {
      const result = await this.fetchJson<unknown>(`${this.searchRoot}?query=${encodeURIComponent(query)}`);
      for (const product of this.products(result)) {
        const key = typeof product.url === 'string' ? product.url : String(product.key ?? product.full_name ?? product.name ?? candidates.size);
        candidates.set(key, product);
      }
      const ranked = [...candidates.values()].map((product) => ({ product, score: this.score(product, target, component.manufacturerPartNumber, component.model) })).sort((a, b) => b.score - a.score);
      if (ranked[0]?.score >= 0.92) return ranked[0].product;
    }
    const best = [...candidates.values()].map((product) => ({ product, score: this.score(product, target, component.manufacturerPartNumber, component.model) })).sort((a, b) => b.score - a.score)[0];
    return best && best.score >= 0.68 ? best.product : null;
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
    for (const key of ['products', 'items', 'results']) if (Array.isArray(record[key])) return record[key].filter((item): item is CatalogProduct => Boolean(item) && typeof item === 'object');
    return [];
  }

  private score(product: CatalogProduct, target: string, mpn: string | null, model: string) {
    const text = this.clean([product.name, product.full_name, product.key, product.html_url].filter((value): value is string => typeof value === 'string').join(' '));
    const part = this.clean(mpn ?? '');
    if (part && part.length >= 5 && text.replace(/ /g, '').includes(part.replace(/ /g, ''))) return 1;
    const modelTokens = this.clean(model).split(' ').filter((word) => word.length > 1);
    const modelScore = modelTokens.length ? modelTokens.filter((word) => text.includes(word)).length / modelTokens.length : 0;
    if (!text || !target) return 0;
    const brand = this.clean(target.split(' ').slice(0, 1).join(' '));
    const brandScore = brand && text.includes(brand) ? 1 : 0;
    // Give the exact model more weight than the brand, but still reject weak matches.
    return modelScore * 0.8 + brandScore * 0.2;
  }

  private price(product: CatalogProduct): number | null {
    const prices = product.prices && typeof product.prices === 'object' ? product.prices as Record<string, unknown> : {};
    const raw = [product.price, prices.price_min, prices.min, prices.amount].find((value) => value != null);
    const candidate = raw && typeof raw === 'object' ? (raw as Record<string, unknown>).amount : raw;
    const value = typeof candidate === 'number' ? candidate : Number(String(candidate ?? '').replace(',', '.'));
    return Number.isFinite(value) && value > 0 ? value : null;
  }

  private images(product: CatalogProduct): string[] {
    const result: string[] = [];
    const collect = (value: unknown) => {
      if (typeof value === 'string' && /^https:\/\//i.test(value)) result.push(value);
      else if (Array.isArray(value)) value.forEach(collect);
      else if (value && typeof value === 'object') {
        const record = value as Record<string, unknown>;
        for (const key of ['url', 'src', 'high', 'highpic', 'header', 'icon', 'image', 'image_url']) {
          if (record[key] != null) collect(record[key]);
        }
      }
    };
    collect(product.images);
    collect(product.image_url);
    collect(product.image);
    return [...new Set(result)];
  }

  private priceOffers(product: CatalogProduct): { shop: string; price: number; url: string | null }[] {
    const source = product.retailer_listings ?? product.offers ?? product.prices;
    const found: { shop: string; price: number; url: string | null }[] = [];
    const visit = (value: unknown, inheritedShop = 'Onliner') => {
      if (Array.isArray(value)) { value.forEach((entry) => visit(entry, inheritedShop)); return; }
      if (!value || typeof value !== 'object') return;
      const record = value as Record<string, unknown>;
      const shop = String(record.shop_name ?? record.retailer ?? record.seller ?? record.name ?? inheritedShop);
      const raw = record.price ?? record.price_min ?? record.amount;
      const candidate = raw && typeof raw === 'object' ? (raw as Record<string, unknown>).amount : raw;
      const price = typeof candidate === 'number' ? candidate : Number(String(candidate ?? '').replace(',', '.'));
      const urlValue = record.html_url ?? record.url ?? record.link;
      if (Number.isFinite(price) && price > 0) {
        found.push({ shop, price, url: typeof urlValue === 'string' && /^https:\/\//i.test(urlValue) ? urlValue : null });
      }
      for (const [key, child] of Object.entries(record)) {
        if (child && typeof child === 'object' && !['image', 'images'].includes(key)) visit(child, key);
      }
    };
    visit(source);
    const unique = new Map<string, { shop: string; price: number; url: string | null }>();
    found.forEach((offer) => unique.set(`${offer.shop}|${offer.price}|${offer.url ?? ''}`, offer));
    return [...unique.values()].sort((a, b) => a.price - b.price).slice(0, 20);
  }

  private htmlUrl(product: CatalogProduct): string | null { return typeof product.html_url === 'string' ? product.html_url : null; }
  private async fetchJson<T>(url: string): Promise<T> { const response = await fetch(url, { headers: { Accept: 'application/json', 'User-Agent': 'PC-Configurator-Educational-Project' }, signal: AbortSignal.timeout(20000) }); if (!response.ok) throw new Error(`HTTP ${response.status}`); return await response.json() as T; }
  private clean(value: string) { return value.toLocaleLowerCase().replace(/[^a-zа-я0-9]+/gi, ' ').trim(); }
  private pause(ms: number) { return new Promise<void>((resolve) => setTimeout(resolve, ms)); }
  private errorText(error: unknown) { return error instanceof Error ? error.message : String(error); }
}
