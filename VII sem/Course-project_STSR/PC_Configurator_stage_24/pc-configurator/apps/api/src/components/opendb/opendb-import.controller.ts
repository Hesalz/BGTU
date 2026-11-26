import { Controller, Post, UseGuards } from '@nestjs/common';
import { JwtAuthGuard } from '../../auth/auth.guard';
import { AdminGuard } from '../../auth/roles.guard';
import { OpenDbImportService } from './opendb-import.service';

@Controller('components/import')
@UseGuards(JwtAuthGuard, AdminGuard)
export class OpenDbImportController {
  constructor(private readonly importer: OpenDbImportService) {}

  @Post('opendb')
  importOpenDb() {
    return this.importer.importCatalog();
  }
}
