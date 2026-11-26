import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { CatalogDataRowDto, ImportCatalogDataDto } from './dto/import-catalog-data.dto';
import { Component } from './entities/component.entity';

@Injectable()
export class CatalogDataImportService {
  constructor(@InjectRepository(Component) private readonly components: Repository<Component>) {}

  async import(dto: ImportCatalogDataDto) {
    const components = await this.components.find();
    let updated = 0; let unmatched = 0; let invalid = 0;
    for (const row of dto.rows) {
      const partNumber = this.key(row.partNumber);
      if (!partNumber) { invalid += 1; continue; }
      const matches = components.filter((item) => this.matches(item, partNumber, row.manufacturer));
      if (matches.length !== 1) { unmatched += 1; continue; }
      const component = matches[0];
      this.apply(component, row, dto.source.trim());
      await this.components.save(component);
      updated += 1;
    }
    return { source: dto.source.trim(), updated, unmatched, invalid };
  }

  private matches(component: Component, partNumber: string, manufacturer?: string) {
    if (manufacturer?.trim() && this.key(component.manufacturer) !== this.key(manufacturer)) return false;
    return this.partNumbers(component).some((value) => this.key(value) === partNumber);
  }

  private apply(component: Component, row: CatalogDataRowDto, source: string) {
    if (row.price !== undefined) { component.price = row.price; component.priceSource = source; component.priceUrl = row.productUrl?.trim() || null; component.priceUpdatedAt = new Date(); }
    if (row.imageUrl?.trim()) { component.imageUrl = row.imageUrl.trim(); component.imageSource = source; }
  }

  private partNumbers(component: Component): string[] {
    const values = [component.manufacturerPartNumber];
    try {
      const parsed = JSON.parse(component.specifications ?? '{}') as { metadata?: { part_numbers?: unknown }; part_numbers?: unknown };
      const source = parsed.metadata?.part_numbers ?? parsed.part_numbers;
      if (Array.isArray(source)) values.push(...source.filter((value): value is string => typeof value === 'string'));
      else if (typeof source === 'string') values.push(source);
    } catch { /* Manually entered specifications may not be JSON. */ }
    return values.filter((value): value is string => Boolean(value?.trim()));
  }

  private key(value: string) { return value.trim().toLocaleLowerCase().replace(/[^a-z0-9]/gi, ''); }
}
