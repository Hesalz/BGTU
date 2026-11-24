import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { Component } from './entities/component.entity';
import { Category } from './entities/category.entity';
import { ComponentsController } from './components.controller';
import { CategoriesController } from './categories.controller';
import { ComponentsService } from './components.service';
import { CategoriesService } from './categories.service';
import { AuthModule } from '../auth/auth.module';
import { OpenDbImportController } from './opendb/opendb-import.controller';
import { OpenDbImportService } from './opendb/opendb-import.service';
import { CompatibilityService } from './compatibility.service';
import { CatalogDataImportService } from './catalog-data-import.service';
import { OnlinerCatalogService } from './onliner-catalog.service';

@Module({
  imports: [TypeOrmModule.forFeature([Component, Category]), AuthModule],
  controllers: [ComponentsController, CategoriesController, OpenDbImportController],
  providers: [ComponentsService, CategoriesService, OpenDbImportService, CompatibilityService, CatalogDataImportService, OnlinerCatalogService],
  exports: [ComponentsService, CategoriesService],
})
export class ComponentsModule {}
