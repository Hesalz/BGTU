import { ArrayMaxSize, IsArray, IsNotEmpty, IsString } from 'class-validator';

export class CheckCompatibilityDto {
  @IsArray()
  @ArrayMaxSize(32)
  // Existing databases can contain UUIDs created by earlier versions or
  // externally supplied stable identifiers. The service resolves only records
  // that actually exist in the catalog, so requiring UUID v4 here is needlessly
  // strict and prevents checking an otherwise valid current build.
  @IsString({ each: true })
  @IsNotEmpty({ each: true })
  componentIds: string[];
}
