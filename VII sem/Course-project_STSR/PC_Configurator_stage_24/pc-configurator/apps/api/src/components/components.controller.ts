import { Body, Controller, Delete, Get, Param, ParseUUIDPipe, Patch, Post, Query, HttpCode, HttpStatus, UseGuards, UploadedFile, UseInterceptors, BadRequestException } from '@nestjs/common';
import { FileInterceptor } from '@nestjs/platform-express';
import { ComponentsService } from './components.service';
import { CreateComponentDto } from './dto/create-component.dto';
import { UpdateComponentDto } from './dto/update-component.dto';
import { QueryComponentsDto } from './dto/query-components.dto';
import { CheckCompatibilityDto } from './dto/check-compatibility.dto';
import { CompatibilityService } from './compatibility.service';
import { CatalogDataImportService } from './catalog-data-import.service';
import { ImportCatalogDataDto } from './dto/import-catalog-data.dto';
import { OnlinerCatalogService } from './onliner-catalog.service';
import { IcecatImagesService } from './icecat-images.service';
import { JwtAuthGuard } from '../auth/auth.guard';
import { AdminGuard } from '../auth/roles.guard';

@Controller('components')
export class ComponentsController {
  constructor(private readonly componentsService: ComponentsService, private readonly compatibilityService: CompatibilityService, private readonly catalogDataImport: CatalogDataImportService, private readonly onlinerCatalog: OnlinerCatalogService, private readonly icecatImages: IcecatImagesService) {}

  @Get()
  findAll(@Query() filters: QueryComponentsDto) {
    return this.componentsService.findAll(filters);
  }

  @Post('compatibility')
  checkCompatibility(@Body() dto: CheckCompatibilityDto) {
    return this.compatibilityService.check(dto.componentIds);
  }

  @Post('import/catalog-data')
  @UseGuards(JwtAuthGuard, AdminGuard)
  importCatalogData(@Body() dto: ImportCatalogDataDto) {
    return this.catalogDataImport.import(dto);
  }

  @Post('import/catalog-file')
  @UseGuards(JwtAuthGuard, AdminGuard)
  @UseInterceptors(FileInterceptor('file', { limits: { fileSize: 5 * 1024 * 1024 } }))
  importCatalogFile(@UploadedFile() file: { originalname: string; mimetype: string; buffer: Buffer } | undefined, @Body('source') source?: string) {
    if (!file) throw new BadRequestException('Выберите CSV- или XLSX-файл прайс-листа.');
    return this.catalogDataImport.importFile(file, source);
  }

  @Post('import/onliner')
  @UseGuards(JwtAuthGuard, AdminGuard)
  importOnliner() {
    return this.onlinerCatalog.enrichCatalog();
  }

  @Post('import/icecat-images')
  @UseGuards(JwtAuthGuard, AdminGuard)
  importIcecatImages() {
    return this.icecatImages.enrichImages();
  }

  @Post('import/catalog-enrichment')
  @UseGuards(JwtAuthGuard, AdminGuard)
  async enrichCatalog() {
    // A missing Icecat account must not prevent the independent Onliner price refresh.
    let icecat: { updated: number; skipped: number; failed: number; error?: string };
    try {
      icecat = await this.icecatImages.enrichImages();
    } catch (error) {
      icecat = {
        updated: 0,
        skipped: 0,
        failed: 0,
        error: error instanceof Error ? error.message : String(error),
      };
    }
    const onliner = await this.onlinerCatalog.enrichCatalog();
    return { onliner, icecat };
  }

  @Get(':id')
  findOne(@Param('id', ParseUUIDPipe) id: string) {
    return this.componentsService.findOne(id);
  }

  @Post()
  @UseGuards(JwtAuthGuard, AdminGuard)
  create(@Body() dto: CreateComponentDto) {
    return this.componentsService.create(dto);
  }

  @Patch(':id')
  @UseGuards(JwtAuthGuard, AdminGuard)
  update(@Param('id', ParseUUIDPipe) id: string, @Body() dto: UpdateComponentDto) {
    return this.componentsService.update(id, dto);
  }

  @Delete(':id')
  @UseGuards(JwtAuthGuard, AdminGuard)
  @HttpCode(HttpStatus.NO_CONTENT)
  remove(@Param('id', ParseUUIDPipe) id: string): Promise<void> {
    return this.componentsService.remove(id);
  }
}
