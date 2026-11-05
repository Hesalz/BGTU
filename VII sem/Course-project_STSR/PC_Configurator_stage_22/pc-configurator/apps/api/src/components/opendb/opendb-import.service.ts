import { BadGatewayException, Injectable, Logger } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Category } from '../entities/category.entity';
import { Component } from '../entities/component.entity';

type OpenDbRecord = {
  opendb_id?: string;
  metadata?: { name?: string; manufacturer?: string; part_numbers?: string[]; series?: string; variant?: string; releaseYear?: number };
  manufacturer?: string;
  chipset_manufacturer?: string;
  chipset?: string;
  series?: string;
  socket?: string;
  cores?: { total?: number; threads?: number };
  memory?: number;
  memory_type?: string;
  storage_type?: string;
  capacity?: number;
  specifications?: { tdp?: number; wattage?: number; [key: string]: unknown };
  tdp?: number;
  wattage?: number;
  general_product_information?: { manufacturer_url?: string };
  [key: string]: unknown;
};

const SOURCE_NAME = 'BuildCores OpenDB';
const CATEGORY_MAP = [
  { folder: 'CPU', slug: 'cpu' },
  { folder: 'GPU', slug: 'gpu' },
  { folder: 'Motherboard', slug: 'motherboard' },
  { folder: 'RAM', slug: 'ram' },
  { folder: 'Storage', slug: 'storage' },
  { folder: 'CPUCooler', slug: 'cooling' },
  { folder: 'PSU', slug: 'psu' },
  { folder: 'PCCase', slug: 'case' },
];
const RECORDS_PER_CATEGORY = 8;
const API_ROOT = 'https://api.github.com/repos/buildcores/buildcores-open-db/contents/open-db';
const RAW_ROOT = 'https://raw.githubusercontent.com/buildcores/buildcores-open-db/main/open-db';

@Injectable()
export class OpenDbImportService {
  private readonly logger = new Logger(OpenDbImportService.name);

  constructor(
    @InjectRepository(Component) private readonly components: Repository<Component>,
    @InjectRepository(Category) private readonly categories: Repository<Category>,
  ) {}

  async importCatalog(): Promise<{ source: string; inserted: number; updated: number; skipped: number; limitPerCategory: number }> {
    const categories = new Map<string, Category>((await this.categories.find()).map((item) => [item.slug, item] as [string, Category]));
    let inserted = 0;
    let updated = 0;
    let skipped = 0;

    for (const mapping of CATEGORY_MAP) {
      const category = categories.get(mapping.slug);
      if (!category) {
        this.logger.warn(`Категория ${mapping.slug} отсутствует; пропуск ${mapping.folder}`);
        skipped += 1;
        continue;
      }

      const listingResponse = await fetch(`${API_ROOT}/${mapping.folder}?ref=main`, {
        headers: { Accept: 'application/vnd.github+json', 'User-Agent': 'PC-Configurator-OpenDB-Importer' },
        signal: AbortSignal.timeout(20000),
      }).catch((error: unknown) => {
        throw new BadGatewayException(`Не удалось связаться с GitHub при получении категории ${mapping.folder}: ${this.errorText(error)}`);
      });
      if (!listingResponse.ok) {
        throw new BadGatewayException(`GitHub вернул HTTP ${listingResponse.status} для категории ${mapping.folder}`);
      }

      const files = await listingResponse.json() as Array<{ name?: string; type?: string }>;
      const selected = files
        .filter((file) => file.type === 'file' && /^[0-9a-f-]{36}\.json$/i.test(file.name ?? ''))
        .slice(0, RECORDS_PER_CATEGORY);

      for (const file of selected) {
        try {
          const externalId = (file.name as string).replace(/\.json$/i, '');
          const dataResponse = await fetch(`${RAW_ROOT}/${mapping.folder}/${file.name}`, {
            headers: { 'User-Agent': 'PC-Configurator-OpenDB-Importer' },
            signal: AbortSignal.timeout(15000),
          });
          if (!dataResponse.ok) { skipped += 1; continue; }
          const data = await dataResponse.json() as OpenDbRecord;
          const name = data.metadata?.name?.trim();
          const manufacturer = (data.metadata?.manufacturer ?? data.manufacturer ?? data.chipset_manufacturer ?? '').trim();
          if (!name || !manufacturer) { skipped += 1; continue; }

          // The JSON filename is the stable OpenDB record identifier. Some source
          // records also contain opendb_id, but relying on that field alone can
          // create duplicates if it differs from the filename or changes format.
          // Search both identifiers to update records imported by older versions,
          // then persist the filename identifier as the canonical externalId.
          const sourceId = externalId;
          const legacySourceId = data.opendb_id?.trim();
          let existing = await this.components.findOne({
            where: { sourceName: SOURCE_NAME, externalId: sourceId },
          });
          if (!existing && legacySourceId && legacySourceId !== sourceId) {
            existing = await this.components.findOne({
              where: { sourceName: SOURCE_NAME, externalId: legacySourceId },
            });
          }
          // powerW represents PSU output power, not CPU/GPU thermal design power.
          const rawPower = mapping.slug === 'psu' ? (data.wattage ?? data.specifications?.wattage) : undefined;
          const powerW = typeof rawPower === 'number' && Number.isFinite(rawPower) && rawPower >= 0 ? Math.round(rawPower) : null;
          const details = this.buildDescription(mapping.slug, data);
          const record = this.components.create({
            ...(existing ? { id: existing.id } : {}),
            category: mapping.slug,
            categoryId: category.id,
            categoryEntity: category,
            manufacturer: manufacturer.slice(0, 120),
            model: name.slice(0, 180),
            description: details.slice(0, 4000),
            price: null,
            powerW,
            specifications: JSON.stringify(data),
            imageUrl: null,
            sourceName: SOURCE_NAME,
            externalId: sourceId.slice(0, 180),
          });
          await this.components.save(record);
          if (existing) updated += 1; else inserted += 1;
        } catch (error) {
          if (error instanceof BadGatewayException) throw error;
          this.logger.warn(`Не удалось импортировать ${mapping.folder}/${file.name}: ${this.errorText(error)}`);
          skipped += 1;
        }
      }
    }

    return { source: SOURCE_NAME, inserted, updated, skipped, limitPerCategory: RECORDS_PER_CATEGORY };
  }

  private buildDescription(category: string, data: OpenDbRecord): string {
    const details: string[] = [];
    const add = (label: string, value: unknown) => {
      if (value === null || value === undefined || value === '' || typeof value === 'object') return;
      details.push(`${label}: ${String(value)}`);
    };

    switch (category) {
      case 'cpu':
        add('Сокет', data.socket);
        add('Ядра', data.cores?.total);
        add('Потоки', data.cores?.threads);
        add('Базовая частота', data.base_clock);
        add('Максимальная частота', data.boost_clock);
        add('Теплопакет', data.tdp ?? data.specifications?.tdp);
        break;
      case 'gpu':
        add('Память видеокарты', typeof data.memory === 'number' ? `${data.memory} ГБ` : data.memory);
        add('Тип памяти', data.memory_type);
        add('Интерфейс', data.interface);
        add('Длина видеокарты', data.length);
        add('Энергопотребление', data.tdp ?? data.specifications?.tdp);
        break;
      case 'motherboard':
        add('Сокет процессора', data.socket);
        add('Чипсет', data.chipset);
        add('Форм-фактор', data.form_factor);
        add('Тип памяти', data.memory_type);
        add('Слоты памяти', data.memory_slots);
        add('Максимальный объём памяти', data.max_memory);
        break;
      case 'ram':
        add('Тип памяти', data.memory_type ?? data.type);
        add('Объём комплекта', typeof data.capacity === 'number' ? `${data.capacity} ГБ` : data.capacity);
        add('Скорость', data.speed);
        add('Форм-фактор', data.form_factor);
        add('CAS Latency', data.cas_latency);
        break;
      case 'storage':
        add('Тип накопителя', data.storage_type);
        add('Ёмкость', typeof data.capacity === 'number' ? `${data.capacity} ГБ` : data.capacity);
        add('Форм-фактор', data.form_factor);
        add('Интерфейс', data.interface);
        add('Скорость чтения', data.sequential_read);
        add('Скорость записи', data.sequential_write);
        break;
      case 'cooling':
        add('Тип охлаждения', data.type);
        add('Совместимые сокеты', Array.isArray(data.socket) ? data.socket.join(', ') : data.socket);
        add('Высота', data.height);
        add('Размер вентилятора', data.fan_size);
        add('Уровень шума', data.noise_level);
        break;
      case 'psu':
        add('Мощность', typeof data.wattage === 'number' ? `${data.wattage} Вт` : data.wattage);
        add('Сертификат эффективности', data.efficiency);
        add('Форм-фактор', data.form_factor);
        add('Модульные кабели', data.modular);
        break;
      case 'case':
        add('Форм-фактор корпуса', data.form_factor);
        add('Поддерживаемые платы', Array.isArray(data.supported_motherboard_form_factors) ? data.supported_motherboard_form_factors.join(', ') : undefined);
        add('Максимальная длина видеокарты', typeof data.max_video_card_length === 'number' ? `${data.max_video_card_length} мм` : data.max_video_card_length);
        add('Максимальная высота кулера', typeof data.max_cpu_cooler_height === 'number' ? `${data.max_cpu_cooler_height} мм` : data.max_cpu_cooler_height);
        add('Отсеки 3,5 дюйма', data.internal_3_5_bays);
        add('Отсеки 2,5 дюйма', data.internal_2_5_bays);
        add('Цвет', Array.isArray(data.color) ? data.color.join(', ') : data.color);
        break;
    }

    if (details.length) return details.join(' · ');
    return 'Подробные характеристики пока не указаны. Проверьте спецификацию на сайте производителя.';
  }

  private errorText(error: unknown): string {
    return error instanceof Error ? error.message : String(error);
  }
}
