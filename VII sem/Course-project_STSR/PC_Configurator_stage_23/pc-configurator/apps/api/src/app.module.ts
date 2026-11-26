import { Module } from '@nestjs/common';
import { ConfigModule, ConfigService } from '@nestjs/config';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ComponentsModule } from './components/components.module';
import { HealthModule } from './health/health.module';
import { Component } from './components/entities/component.entity';
import { Category } from './components/entities/category.entity';
import { User } from './auth/user.entity';
import { AuthModule } from './auth/auth.module';
import { Configuration } from './configurations/configuration.entity';
import { ConfigurationsModule } from './configurations/configurations.module';

@Module({
  imports: [
    ConfigModule.forRoot({ isGlobal: true }),
    TypeOrmModule.forRootAsync({
      imports: [ConfigModule],
      inject: [ConfigService],
      useFactory: (config: ConfigService) => ({
        type: 'mssql' as const,
        host: config.get<string>('DB_HOST', 'localhost'),
        port: Number(config.get<string>('DB_PORT', '1433')),
        username: config.get<string>('DB_USERNAME', 'sa'),
        password: config.get<string>('DB_PASSWORD', ''),
        database: config.get<string>('DB_DATABASE', 'PCConfigurator'),
        entities: [Component, Category, User, Configuration],
        synchronize: config.get<string>('DB_SYNCHRONIZE', 'false') === 'true',
        options: {
          encrypt: true,
          trustServerCertificate: true,
        },
        connectionTimeout: 30000,
        requestTimeout: 30000,
      }),
    }),
    HealthModule,
    AuthModule,
    ComponentsModule,
    ConfigurationsModule,
  ],
})
export class AppModule {}
