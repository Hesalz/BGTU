/*
  Stage 10 catalogue enrichment migration.
  Run once against the PCConfigurator database before starting the updated API.
*/
IF COL_LENGTH('dbo.components', 'image_urls') IS NULL
  ALTER TABLE dbo.components ADD image_urls NVARCHAR(MAX) NULL;
GO
IF COL_LENGTH('dbo.components', 'price_offers') IS NULL
  ALTER TABLE dbo.components ADD price_offers NVARCHAR(MAX) NULL;
GO
UPDATE dbo.components
SET image_urls = N'["' + STRING_ESCAPE(image_url, 'json') + N'"]'
WHERE image_url IS NOT NULL
  AND (image_urls IS NULL OR LTRIM(RTRIM(image_urls)) = N'');
GO
