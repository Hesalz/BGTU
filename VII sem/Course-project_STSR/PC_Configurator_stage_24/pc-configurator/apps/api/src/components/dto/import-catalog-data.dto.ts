import { Type } from 'class-transformer';
import {
  ArrayMaxSize, IsArray, IsNumber, IsOptional, IsString, MaxLength, Min,
  ValidateNested,
} from 'class-validator';

export class CatalogPriceOfferDto {
  @IsString()
  @MaxLength(180)
  shop: string;

  @Type(() => Number)
  @IsNumber({ maxDecimalPlaces: 2 })
  @Min(0.01)
  price: number;

  @IsOptional()
  @IsString()
  @MaxLength(1000)
  url?: string;
}

export class CatalogDataRowDto {
  @IsString()
  @MaxLength(180)
  partNumber: string;

  @IsOptional()
  @IsString()
  @MaxLength(120)
  manufacturer?: string;

  @IsOptional()
  @Type(() => Number)
  @IsNumber({ maxDecimalPlaces: 2 })
  @Min(0)
  price?: number;

  @IsOptional()
  @IsString()
  @MaxLength(1000)
  productUrl?: string;

  @IsOptional()
  @IsString()
  @MaxLength(1000)
  imageUrl?: string;

  @IsOptional()
  @IsArray()
  @ArrayMaxSize(12)
  @IsString({ each: true })
  imageUrls?: string[];

  @IsOptional()
  @IsArray()
  @ArrayMaxSize(20)
  @ValidateNested({ each: true })
  @Type(() => CatalogPriceOfferDto)
  priceOffers?: CatalogPriceOfferDto[];
}

export class ImportCatalogDataDto {
  @IsString()
  @MaxLength(120)
  source: string;

  @IsArray()
  @ArrayMaxSize(1000)
  @ValidateNested({ each: true })
  @Type(() => CatalogDataRowDto)
  rows: CatalogDataRowDto[];
}
