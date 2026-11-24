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
    let updated = 0; let skipped = 0; let failed = 0;
    for (const component of components) {
      try {
        const product = await this.findProduct(component);
        if (!product) { skipped += 1; continue; }
        const details = await this.details(product);
        const price = this.price(details); const imageUrl = this.image(details) ?? this.image(product);
        if (price == null && !imageUrl) { skipped += 1; continue; }
        if (price != null) { component.price = price; component.priceSource = 'Каталог Onliner'; component.priceUrl = this.htmlUrl(product); component.priceUpdatedAt = new Date(); }
        if (imageUrl) { component.imageUrl = imageUrl; component.imageSource = 'Каталог Onliner'; }
        await this.components.save(component); updated += 1;
      } catch (error) {
        failed += 1;
        this.logger.warn(`Не удалось обновить ${component.manufacturer} ${component.model}: ${this.errorText(error)}`);
      }
      // Respect the catalogue: one request sequence at a time, with a short pause.
      await this.pause(350);
    }
    return { source: 'Каталог Onliner', updated, skipped, failed };
  }

  private async findProduct(component: Component): Promise<CatalogProduct | null> {
    const query = [component.manufacturerPartNumber, component.manufacturer, component.model].filter(Boolean).join(' ');
    if (!query.trim()) return null;
    const result = await this.fetchJson<unknown>(`${this.searchRoot}?query=${encodeURIComponent(query)}`);
    const candidates = this.products(result);
    const target = this.clean([component.manufacturerPartNumber, component.manufacturer, component.model].filter(Boolean).join(' '));
    const best = candidates.map((product) => ({ product, score: this.score(product, target, component.manufacturerPartNumber) })).sort((a, b) => b.score - a.score)[0];
    // Require a strong match to prevent assigning a price to a neighbouring model.
    return best && best.score >= 0.82 ? best.product : null;
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

  private score(product: CatalogProduct, target: string, mpn: string | null) {
    const text = this.clean([product.name, product.full_name, product.key, product.html_url].filter((value): value is string => typeof value === 'string').join(' '));
    const part = this.clean(mpn ?? '');
    if (part && text.includes(part)) return 1;
    if (!text || !target) return 0;
    const words = target.split(' ').filter((word) => word.length > 2);
    return words.length ? words.filter((word) => text.includes(word)).length / words.length : 0;
  }

  private price(product: CatalogProduct): number | null {
    const prices = product.prices && typeof product.prices === 'object' ? product.prices as Record<string, unknown> : {};
    const raw = [product.price, prices.price_min, prices.min, prices.amount].find((value) => value != null);
    const candidate = raw && typeof raw === 'object' ? (raw as Record<string, unknown>).amount : raw;
    const value = typeof candidate === 'number' ? candidate : Number(String(candidate ?? '').replace(',', '.'));
    return Number.isFinite(value) && value >= 0 ? value : null;
  }

  private image(product: CatalogProduct): string | null {
    const images = product.images && typeof product.images === 'object' ? product.images as Record<string, unknown> : {};
    const value = images.header ?? images.icon ?? product.image_url;
    return typeof value === 'string' && /^https:\/\//i.test(value) ? value : null;
  }

  private htmlUrl(product: CatalogProduct): string | null { return typeof product.html_url === 'string' ? product.html_url : null; }
  private async fetchJson<T>(url: string): Promise<T> { const response = await fetch(url, { headers: { Accept: 'application/json', 'User-Agent': 'PC-Configurator-Educational-Project' }, signal: AbortSignal.timeout(20000) }); if (!response.ok) throw new Error(`HTTP ${response.status}`); return await response.json() as T; }
  private clean(value: string) { return value.toLocaleLowerCase().replace(/[^a-zа-я0-9]+/gi, ' ').trim(); }
  private pause(ms: number) { return new Promise<void>((resolve) => setTimeout(resolve, ms)); }
  private errorText(error: unknown) { return error instanceof Error ? error.message : String(error); }
}
