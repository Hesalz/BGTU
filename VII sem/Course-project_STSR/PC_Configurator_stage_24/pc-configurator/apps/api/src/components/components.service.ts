import { BadRequestException, Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Component } from './entities/component.entity';
import { Category } from './entities/category.entity';
import { CreateComponentDto } from './dto/create-component.dto';
import { UpdateComponentDto } from './dto/update-component.dto';
import { QueryComponentsDto } from './dto/query-components.dto';

@Injectable()
export class ComponentsService {
  constructor(
    @InjectRepository(Component) private readonly componentsRepository: Repository<Component>,
    @InjectRepository(Category) private readonly categoriesRepository: Repository<Category>,
  ) {}

  async findAll(filters: QueryComponentsDto): Promise<Component[]> {
    const query = this.componentsRepository.createQueryBuilder('component')
      .leftJoinAndSelect('component.categoryEntity', 'category')
      .orderBy('component.category', 'ASC')
      .addOrderBy('component.manufacturer', 'ASC')
      .addOrderBy('component.model', 'ASC');

    if (filters.categoryId) query.andWhere('component.categoryId = :categoryId', { categoryId: filters.categoryId });
    if (filters.search?.trim()) {
      query.andWhere('(component.model LIKE :search OR component.manufacturer LIKE :search OR component.description LIKE :search)',
        { search: `%${filters.search.trim()}%` });
    }
    if (filters.manufacturer?.trim()) query.andWhere('component.manufacturer LIKE :manufacturer', { manufacturer: `%${filters.manufacturer.trim()}%` });
    if (filters.minPrice !== undefined) query.andWhere('component.price >= :minPrice', { minPrice: filters.minPrice });
    if (filters.maxPrice !== undefined) query.andWhere('component.price <= :maxPrice', { maxPrice: filters.maxPrice });
    return query.take(200).getMany();
  }

  async findOne(id: string): Promise<Component> {
    const component = await this.componentsRepository.findOne({ where: { id }, relations: { categoryEntity: true } });
    if (!component) throw new NotFoundException(`Комплектующая с идентификатором ${id} не найдена`);
    return component;
  }

  async create(dto: CreateComponentDto): Promise<Component> {
    const category = await this.categoriesRepository.findOne({ where: { id: dto.categoryId } });
    if (!category) throw new BadRequestException('Указанная категория не существует');
    const { categoryId: _categoryId, ...fields } = dto;
    const component = this.componentsRepository.create({
      ...fields,
      category: category.slug,
      categoryId: category.id,
      categoryEntity: category,
    });
    return this.componentsRepository.save(component);
  }

  async update(id: string, dto: UpdateComponentDto): Promise<Component> {
    const component = await this.findOne(id);
    if (dto.categoryId) {
      const category = await this.categoriesRepository.findOne({ where: { id: dto.categoryId } });
      if (!category) throw new BadRequestException('Указанная категория не существует');
      component.categoryId = category.id;
      component.category = category.slug;
      component.categoryEntity = category;
    }
    const { categoryId: _categoryId, ...fields } = dto;
    Object.assign(component, fields);
    return this.componentsRepository.save(component);
  }

  async remove(id: string): Promise<void> {
    const component = await this.findOne(id);
    await this.componentsRepository.remove(component);
  }
}
