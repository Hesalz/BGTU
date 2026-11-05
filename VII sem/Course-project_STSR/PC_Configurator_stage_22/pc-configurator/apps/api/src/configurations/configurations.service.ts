import { BadRequestException, Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { In, Repository } from 'typeorm';
import { Component } from '../components/entities/component.entity';
import { CreateConfigurationDto } from './dto/create-configuration.dto';
import { Configuration } from './configuration.entity';

@Injectable()
export class ConfigurationsService {
  constructor(
    @InjectRepository(Configuration) private readonly configurations: Repository<Configuration>,
    @InjectRepository(Component) private readonly components: Repository<Component>,
  ) {}

  async findMine(ownerId: string) {
    const rows = await this.configurations.find({ where: { ownerId }, order: { updatedAt: 'DESC' } });
    return Promise.all(rows.map((row) => this.toResponse(row)));
  }

  async create(ownerId: string, dto: CreateConfigurationDto) {
    const ids = [...new Set(dto.componentIds)];
    const found = await this.components.find({ where: { id: In(ids) }, relations: { categoryEntity: true } });
    if (found.length !== ids.length) throw new BadRequestException('Одна или несколько комплектующих больше не существуют');
    const row = this.configurations.create({ ownerId, name: dto.name.trim(), componentIds: JSON.stringify(ids) });
    return this.toResponse(await this.configurations.save(row));
  }

  async update(ownerId: string, id: string, dto: CreateConfigurationDto) {
    const row = await this.configurations.findOne({ where: { id, ownerId } });
    if (!row) throw new NotFoundException('Конфигурация не найдена');
    const ids = [...new Set(dto.componentIds)];
    const found = await this.components.find({ where: { id: In(ids) }, relations: { categoryEntity: true } });
    if (found.length !== ids.length) throw new BadRequestException('Одна или несколько комплектующих больше не существуют');
    row.name = dto.name.trim();
    row.componentIds = JSON.stringify(ids);
    return this.toResponse(await this.configurations.save(row));
  }

  async remove(ownerId: string, id: string): Promise<void> {
    const row = await this.configurations.findOne({ where: { id, ownerId } });
    if (!row) throw new NotFoundException('Конфигурация не найдена');
    await this.configurations.remove(row);
  }

  private async toResponse(row: Configuration) {
    const ids: string[] = JSON.parse(row.componentIds);
    const found = await this.components.find({ where: { id: In(ids) }, relations: { categoryEntity: true } });
    const byId = new Map(found.map((component) => [component.id, component]));
    const components = ids.map((id) => byId.get(id)).filter((component): component is Component => Boolean(component));
    return {
      id: row.id,
      name: row.name,
      createdAt: row.createdAt,
      updatedAt: row.updatedAt,
      components,
      totalPrice: components.reduce((sum, component) => sum + Number(component.price ?? 0), 0),
      totalPowerW: components.reduce((sum, component) => sum + Number(component.powerW ?? 0), 0),
    };
  }
}
