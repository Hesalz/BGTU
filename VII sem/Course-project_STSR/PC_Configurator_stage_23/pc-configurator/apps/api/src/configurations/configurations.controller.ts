import { Body, Controller, Delete, Get, Param, ParseUUIDPipe, Patch, Post, Req, UseGuards, HttpCode, HttpStatus } from '@nestjs/common';
import { AuthRequest } from '../auth/auth.types';
import { JwtAuthGuard } from '../auth/auth.guard';
import { CreateConfigurationDto } from './dto/create-configuration.dto';
import { ConfigurationsService } from './configurations.service';

@Controller('configurations')
@UseGuards(JwtAuthGuard)
export class ConfigurationsController {
  constructor(private readonly configurations: ConfigurationsService) {}

  @Get()
  findMine(@Req() request: AuthRequest) {
    return this.configurations.findMine(request.user!.sub);
  }

  @Post()
  create(@Req() request: AuthRequest, @Body() dto: CreateConfigurationDto) {
    return this.configurations.create(request.user!.sub, dto);
  }

  @Patch(':id')
  update(@Req() request: AuthRequest, @Param('id', ParseUUIDPipe) id: string, @Body() dto: CreateConfigurationDto) {
    return this.configurations.update(request.user!.sub, id, dto);
  }

  @Delete(':id')
  @HttpCode(HttpStatus.NO_CONTENT)
  remove(@Req() request: AuthRequest, @Param('id', ParseUUIDPipe) id: string): Promise<void> {
    return this.configurations.remove(request.user!.sub, id);
  }
}
