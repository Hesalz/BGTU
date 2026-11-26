import { BadRequestException, Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { inflateRawSync } from 'node:zlib';
import { posix } from 'node:path';
import { Repository } from 'typeorm';
import { CatalogDataRowDto, ImportCatalogDataDto } from './dto/import-catalog-data.dto';
import { Component } from './entities/component.entity';
import type ExcelJS from 'exceljs';

type ImportedOffer = { shop: string; price: number; url: string | null; source?: string };

@Injectable()
export class CatalogDataImportService {
  constructor(@InjectRepository(Component) private readonly components: Repository<Component>) {}

  async import(dto: ImportCatalogDataDto) {
    const components = await this.components.find();
    const source = dto.source.trim();
    let updated = 0;
    let unmatched = 0;
    let invalid = 0;
    const offersClearedFor = new Set<string>();

    for (const row of dto.rows) {
      const partNumber = this.key(row.partNumber ?? '');
      if (!partNumber) { invalid += 1; continue; }
      const matches = components.filter((item) => this.matches(item, partNumber, row.manufacturer));
      if (matches.length !== 1) { unmatched += 1; continue; }

      const component = matches[0];
      this.apply(component, row, source, offersClearedFor);
      await this.components.save(component);
      updated += 1;
    }
    return { source, updated, unmatched, invalid };
  }

  async importFile(file: { originalname: string; mimetype: string; buffer: Buffer }, sourceOverride?: string) {
    if (!file?.buffer?.length) throw new BadRequestException('Файл прайса пуст или не был передан.');
    const extension = file.originalname.toLowerCase().split('.').pop();
    if (!['csv', 'xlsx'].includes(extension ?? '')) {
      throw new BadRequestException('Поддерживаются только файлы CSV и XLSX.');
    }

    let matrix: string[][];
    try {
      matrix = extension === 'xlsx'
        ? this.readXlsx(file.buffer)
        : this.readCsv(file.buffer.toString('utf8'));
    } catch {
      throw new BadRequestException('Не удалось прочитать файл. Проверьте формат CSV/XLSX и убедитесь, что файл не повреждён.');
    }

    if (matrix.length < 2) throw new BadRequestException('В файле нет строк с данными.');
    const headerRowIndex = matrix.slice(0, 10).findIndex((row) =>
      row.some((value) => ['partnumber', 'mpn', 'article', 'артикул', 'артикулпроизводителя', 'кодпроизводителя'].includes(this.header(value))));
    if (headerRowIndex < 0) {
      throw new BadRequestException('Не найдена обязательная колонка артикула: partNumber, MPN или Артикул.');
    }
    const headerMap = new Map<string, number>();
    matrix[headerRowIndex].forEach((value, index) => headerMap.set(this.header(value), index));
    const column = (...names: string[]) => names.map((name) => headerMap.get(this.header(name))).find((index) => index !== undefined);

    const partColumn = column('partNumber', 'mpn', 'article', 'артикул', 'артикул производителя', 'код производителя');
    if (partColumn === undefined) {
      throw new BadRequestException('Не найдена обязательная колонка артикула: partNumber, MPN или Артикул.');
    }
    const manufacturerColumn = column('manufacturer', 'brand', 'производитель', 'бренд', 'производитель товара');
    const priceColumn = column('price', 'priceByn', 'цена', 'цена byn');
    const productUrlColumn = column('productUrl', 'url', 'ссылка', 'ссылка на товар');
    const imageColumn = column('imageUrl', 'image', 'photo', 'изображение', 'фото');
    const imageUrlsColumn = column('imageUrls', 'gallery', 'галерея', 'фотографии');
    const shopColumn = column('shop', 'shopName', 'store', 'магазин', 'продавец', 'seller');
    const offerUrlColumn = column('offerUrl', 'offer url', 'ссылка на предложение');

    const rows: CatalogDataRowDto[] = [];
    for (const values of matrix.slice(headerRowIndex + 1)) {
      const cell = (index: number | undefined) => index === undefined ? '' : (values[index] ?? '').trim();
      const partNumber = cell(partColumn);
      if (!partNumber) continue;

      const manufacturer = cell(manufacturerColumn) || undefined;
      const price = this.parsePrice(cell(priceColumn));
      const productUrl = cell(productUrlColumn) || undefined;
      const imageUrl = cell(imageColumn) || undefined;
      const imageUrls = cell(imageUrlsColumn).split(/[;|\n]/).map((value) => value.trim()).filter(Boolean).slice(0, 12);
      const shop = cell(shopColumn);
      const offerUrl = cell(offerUrlColumn) || productUrl;

      const item: CatalogDataRowDto = { partNumber, manufacturer, productUrl, imageUrl, imageUrls };
      if (shop && price != null) item.priceOffers = [{ shop, price, url: offerUrl }];
      else if (price != null) item.price = price;
      rows.push(item);
    }

    if (!rows.length) throw new BadRequestException('В файле не найдено ни одной строки с артикулом.');
    return this.import({ source: (sourceOverride?.trim() || `Прайс-лист: ${file.originalname}`).slice(0, 120), rows });
  }

  /** RFC-4180-compatible lightweight CSV reader; supports comma and semicolon separators. */
  private readCsv(text: string): string[][] {
    const normalized = text.replace(/^\uFEFF/, '');
    const sample = normalized.split(/\r?\n/).slice(0, 10).join('\n');
    const semicolons = (sample.match(/;/g) ?? []).length;
    const commas = (sample.match(/,/g) ?? []).length;
    const delimiter = semicolons > commas ? ';' : ',';
    const rows: string[][] = [];
    let row: string[] = [];
    let value = '';
    let quoted = false;

    for (let index = 0; index < normalized.length; index += 1) {
      const char = normalized[index];
      if (char === '"') {
        if (quoted && normalized[index + 1] === '"') { value += '"'; index += 1; }
        else quoted = !quoted;
      } else if (!quoted && char === delimiter) {
        row.push(value); value = '';
      } else if (!quoted && (char === '\n' || char === '\r')) {
        if (char === '\r' && normalized[index + 1] === '\n') index += 1;
        row.push(value); value = '';
        if (row.some((cell) => cell.trim())) rows.push(row);
        row = [];
      } else value += char;
    }
    row.push(value);
    if (row.some((cell) => cell.trim())) rows.push(row);
    return rows;
  }

  /** Reads the first worksheet from an XLSX ZIP container using Node built-ins. */
  private readXlsx(buffer: Buffer): string[][] {
    const files = this.unzipEntries(buffer);
    const workbookXml = files.get('xl/workbook.xml');
    const relationshipsXml = files.get('xl/_rels/workbook.xml.rels');
    if (!workbookXml || !relationshipsXml) throw new Error('XLSX workbook metadata is missing');

    const sheetTag = workbookXml.match(/<sheet\b[^>]*>/i)?.[0];
    const relationId = sheetTag?.match(/\br:id=["']([^"']+)["']/i)?.[1];
    if (!relationId) throw new Error('XLSX first worksheet is missing');
    const relation = (relationshipsXml.match(/<Relationship\b[^>]*>/gi) ?? [])
      .find((tag) => new RegExp(`\\bId=["']${this.escapeRegExp(relationId)}["']`, 'i').test(tag));
    const target = relation?.match(/\bTarget=["']([^"']+)["']/i)?.[1];
    if (!target) throw new Error('XLSX worksheet relationship is missing');
    const sheetPath = posix.normalize(target.startsWith('/') ? target.slice(1) : posix.join('xl', target));
    const sheetXml = files.get(sheetPath);
    if (!sheetXml) throw new Error('XLSX worksheet content is missing');

    const sharedStringsXml = files.get('xl/sharedStrings.xml') ?? '';
    const sharedStrings = [...sharedStringsXml.matchAll(/<si\b[^>]*>([\s\S]*?)<\/si>/gi)]
      .map((match) => this.xmlText(match[1]));
    const rows: string[][] = [];

    for (const rowMatch of sheetXml.matchAll(/<row\b[^>]*>([\s\S]*?)<\/row>/gi)) {
      const rowValues: string[] = [];
      let nextColumn = 0;
      for (const cellMatch of rowMatch[1].matchAll(/<c\b([^>]*?)(?:\/>|>([\s\S]*?)<\/c>)/gi)) {
        const attributes = cellMatch[1];
        const content = cellMatch[2] ?? '';
        const reference = attributes.match(/\br=["']([A-Z]+)\d+["']/i)?.[1]?.toUpperCase();
        const columnIndex = reference ? this.columnIndex(reference) : nextColumn;
        nextColumn = columnIndex + 1;
        const type = attributes.match(/\bt=["']([^"']+)["']/i)?.[1];
        const rawValue = content.match(/<v\b[^>]*>([\s\S]*?)<\/v>/i)?.[1] ?? '';
        let value = '';
        if (type === 's') value = sharedStrings[Number(rawValue)] ?? '';
        else if (type === 'inlineStr') value = this.xmlText(content);
        else value = this.decodeXml(rawValue);
        rowValues[columnIndex] = value;
      }
      rows.push(rowValues.map((value) => value ?? ''));
    }
    return rows;
  }

  private unzipEntries(buffer: Buffer): Map<string, string> {
    const signature = Buffer.from([0x50, 0x4b, 0x05, 0x06]);
    const endOffset = buffer.lastIndexOf(signature);
    if (endOffset < 0) throw new Error('ZIP end record not found');
    const entriesCount = buffer.readUInt16LE(endOffset + 10);
    if (entriesCount > 2000) throw new Error('XLSX archive contains too many entries');
    let centralOffset = buffer.readUInt32LE(endOffset + 16);
    const files = new Map<string, string>();
    let expandedTotal = 0;

    for (let index = 0; index < entriesCount; index += 1) {
      if (buffer.readUInt32LE(centralOffset) !== 0x02014b50) throw new Error('Invalid ZIP central directory');
      const method = buffer.readUInt16LE(centralOffset + 10);
      const compressedSize = buffer.readUInt32LE(centralOffset + 20);
      const fileNameLength = buffer.readUInt16LE(centralOffset + 28);
      const extraLength = buffer.readUInt16LE(centralOffset + 30);
      const commentLength = buffer.readUInt16LE(centralOffset + 32);
      const localOffset = buffer.readUInt32LE(centralOffset + 42);
      const fileName = buffer.toString('utf8', centralOffset + 46, centralOffset + 46 + fileNameLength);
      centralOffset += 46 + fileNameLength + extraLength + commentLength;

      if (fileName.endsWith('/')) continue;
      if (buffer.readUInt32LE(localOffset) !== 0x04034b50) throw new Error('Invalid ZIP local header');
      const localNameLength = buffer.readUInt16LE(localOffset + 26);
      const localExtraLength = buffer.readUInt16LE(localOffset + 28);
      const dataStart = localOffset + 30 + localNameLength + localExtraLength;
      const compressed = buffer.subarray(dataStart, dataStart + compressedSize);
      const data = method === 0
        ? compressed
        : method === 8 ? inflateRawSync(compressed, { maxOutputLength: 20 * 1024 * 1024 }) : null;
      if (!data) throw new Error(`Unsupported ZIP compression method: ${method}`);
      expandedTotal += data.length;
      if (expandedTotal > 25 * 1024 * 1024) throw new Error('Expanded XLSX contents exceed the import limit');
      files.set(fileName, data.toString('utf8'));
    }
    return files;
  }

  private xmlText(value: string): string {
    return [...value.matchAll(/<t\b[^>]*>([\s\S]*?)<\/t>/gi)]
      .map((match) => this.decodeXml(match[1]))
      .join('');
  }

  private decodeXml(value: string): string {
    return value.replace(/&amp;/g, '&').replace(/&quot;/g, '"')
      .replace(/&apos;/g, "'").replace(/&lt;/g, '<').replace(/&gt;/g, '>');
  }

  private columnIndex(letters: string): number {
    return [...letters].reduce((index, letter) => index * 26 + letter.charCodeAt(0) - 64, 0) - 1;
  }

  private escapeRegExp(value: string): string {
    return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  }

  private matches(component: Component, partNumber: string, manufacturer?: string) {
    if (manufacturer?.trim() && this.key(component.manufacturer) !== this.key(manufacturer)) return false;
    return this.partNumbers(component).some((value) => this.key(value) === partNumber);
  }

  private apply(component: Component, row: CatalogDataRowDto, source: string, offersClearedFor: Set<string>) {
    if (row.price !== undefined) {
      component.price = row.price;
      component.priceSource = source;
      component.priceUrl = row.productUrl?.trim() || null;
      component.priceUpdatedAt = new Date();
    }

    const incomingOffers: ImportedOffer[] = (row.priceOffers ?? [])
      .filter((offer) => Number.isFinite(Number(offer.price)) && Number(offer.price) > 0)
      .map((offer) => ({ shop: offer.shop.trim(), price: Number(offer.price), url: offer.url?.trim() || row.productUrl?.trim() || null, source }));
    if (incomingOffers.length) {
      const existing = this.parseOffers(component.priceOffers);
      const retained = offersClearedFor.has(component.id)
        ? existing
        : existing.filter((offer) => offer.source !== source);
      offersClearedFor.add(component.id);
      const merged = [...retained, ...incomingOffers]
        .filter((offer, index, all) => all.findIndex((candidate) =>
          candidate.shop === offer.shop && candidate.price === offer.price && candidate.url === offer.url && candidate.source === offer.source) === index)
        .sort((a, b) => a.price - b.price)
        .slice(0, 60);
      component.priceOffers = JSON.stringify(merged);
      const sourceOffers = merged.filter((offer) => offer.source === source);
      const cheapest = sourceOffers.reduce((best, offer) => offer.price < best.price ? offer : best, sourceOffers[0]);
      if (cheapest) {
        component.price = cheapest.price;
        component.priceSource = source;
        component.priceUrl = cheapest.url;
        component.priceUpdatedAt = new Date();
      }
    }

    const incomingImages = [...new Set([
      ...(row.imageUrl?.trim() ? [row.imageUrl.trim()] : []),
      ...(row.imageUrls ?? []).map((url) => url.trim()),
    ].filter((url) => /^https?:\/\//i.test(url)))].slice(0, 12);

    if (incomingImages.length) {
      const existingFull = [...new Set([...this.parseUrls(component.imageUrls), ...(component.imageUrl ? [component.imageUrl] : [])])];
      const existingPreview = [...new Set([...this.parseUrls(component.imagePreviewUrls), ...(component.imagePreviewUrl ? [component.imagePreviewUrl] : [])])];
      const preferExisting = component.imageSource === 'Open Icecat';
      const full = [...new Set([...(preferExisting ? existingFull : []), ...incomingImages, ...(preferExisting ? [] : existingFull)])].slice(0, 12);
      const previews = [...new Set([...(preferExisting ? existingPreview : []), ...incomingImages, ...(preferExisting ? [] : existingPreview)])].slice(0, 12);
      component.imageUrl = full[0] ?? component.imageUrl;
      component.imageUrls = JSON.stringify(full);
      component.imagePreviewUrl = previews[0] ?? component.imagePreviewUrl ?? full[0];
      component.imagePreviewUrls = JSON.stringify(previews.length ? previews : full);
      if (!preferExisting) component.imageSource = source;
    }
  }

  private parseOffers(value: string | null): ImportedOffer[] {
    if (!value) return [];
    try {
      const parsed: unknown = JSON.parse(value);
      if (!Array.isArray(parsed)) return [];
      return parsed.filter((offer): offer is ImportedOffer =>
        Boolean(offer) && typeof offer.shop === 'string' && Number.isFinite(Number(offer.price)));
    } catch { return []; }
  }

  private parseUrls(value: string | null): string[] {
    if (!value) return [];
    try {
      const parsed: unknown = JSON.parse(value);
      return Array.isArray(parsed) ? parsed.filter((url): url is string => typeof url === 'string' && /^https?:\/\//i.test(url)) : [];
    } catch { return []; }
  }

  private partNumbers(component: Component): string[] {
    const values = [component.manufacturerPartNumber];
    try {
      const parsed = JSON.parse(component.specifications ?? '{}') as { metadata?: { part_numbers?: unknown }; part_numbers?: unknown };
      const source = parsed.metadata?.part_numbers ?? parsed.part_numbers;
      if (Array.isArray(source)) values.push(...source.filter((value): value is string => typeof value === 'string'));
      else if (typeof source === 'string') values.push(source);
    } catch { /* Manually entered specification text may not be JSON. */ }
    return values.filter((value): value is string => Boolean(value?.trim()));
  }

  private header(value: string) { return value.trim().toLocaleLowerCase().replace(/[\s_-]+/g, ''); }
  private key(value: string) { return value.trim().toLocaleLowerCase().replace(/[^a-z0-9а-я]/gi, ''); }
  private parsePrice(value: string): number | null {
    let normalized = value.replace(/[^0-9,.]/g, '');
    if (normalized.includes(',') && normalized.includes('.')) {
      const decimalSeparator = normalized.lastIndexOf(',') > normalized.lastIndexOf('.') ? ',' : '.';
      const thousandsSeparator = decimalSeparator === ',' ? '.' : ',';
      normalized = normalized.split(thousandsSeparator).join('').replace(decimalSeparator, '.');
    } else {
      normalized = normalized.replace(',', '.');
    }
    const parsed = Number(normalized);
    return Number.isFinite(parsed) && parsed > 0 ? parsed : null;
  }
  private cellText(value: ExcelJS.CellValue | undefined): string {
    if (value == null) return '';
    if (typeof value === 'object') {
      if ('text' in value && typeof value.text === 'string') return value.text;
      if ('result' in value && value.result != null) return String(value.result);
      if ('richText' in value && Array.isArray(value.richText)) return value.richText.map((part) => part.text).join('');
      return String(value);
    }
    return String(value);
  }
}
