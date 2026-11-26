import { Controller, Get } from '@nestjs/common';

@Controller('health')
export class HealthController {
  @Get()
  getHealth(): { status: string; service: string; timestamp: string } {
    return {
      status: 'ok',
      service: 'pc-configurator-api',
      timestamp: new Date().toISOString(),
    };
  }
}
