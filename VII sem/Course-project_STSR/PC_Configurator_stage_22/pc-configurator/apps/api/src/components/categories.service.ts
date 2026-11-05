import { Injectable, OnModuleInit } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Category } from './entities/category.entity';

const DEFAULT_CATEGORIES = [
  { slug: 'cpu', name: 'Процессор', description: 'Центральные процессоры' },
  { slug: 'gpu', name: 'Видеокарта', description: 'Графические ускорители' },
  { slug: 'motherboard', name: 'Материнская плата', description: 'Материнские платы и платформы' },
  { slug: 'ram', name: 'Оперативная память', description: 'Модули оперативной памяти' },
  { slug: 'storage', name: 'Накопители', description: 'SSD и HDD накопители' },
  { slug: 'cooling', name: 'Охлаждение', description: 'Кулеры и системы охлаждения' },
  { slug: 'psu', name: 'Блок питания', description: 'Блоки питания компьютера' },
  { slug: 'case', name: 'Корпус', description: 'Корпуса для компьютера' },
];

@Injectable()
export class CategoriesService implements OnModuleInit {
  constructor(@InjectRepository(Category) private readonly repository: Repository<Category>) {}

  async onModuleInit(): Promise<void> {
    const count = await this.repository.count();
    if (count === 0) await this.repository.save(DEFAULT_CATEGORIES.map((item) => this.repository.create(item)));
  }

  findAll(): Promise<Category[]> {
    return this.repository.find({ order: { name: 'ASC' } });
  }
}
