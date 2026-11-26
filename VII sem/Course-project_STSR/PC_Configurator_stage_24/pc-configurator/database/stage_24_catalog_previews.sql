/*
  Stage 24 — preview image URLs for catalog cards.
  Safe to run more than once. TypeORM synchronization also creates these
  columns when DB_SYNCHRONIZE=true.
*/
IF COL_LENGTH('dbo.components', 'image_preview_url') IS NULL
BEGIN
  ALTER TABLE dbo.components ADD image_preview_url nvarchar(1000) NULL;
END;

IF COL_LENGTH('dbo.components', 'image_preview_urls') IS NULL
BEGIN
  ALTER TABLE dbo.components ADD image_preview_urls nvarchar(MAX) NULL;
END;

UPDATE dbo.components
SET image_preview_url = image_url
WHERE image_preview_url IS NULL AND image_url IS NOT NULL;

UPDATE dbo.components
SET image_preview_urls = image_urls
WHERE image_preview_urls IS NULL AND image_urls IS NOT NULL;
