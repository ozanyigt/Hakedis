-- Unit / WorkType: nvarchar -> int (enum migration'ları)
-- sqlcmd tek batch'te ADD + UPDATE birlikte çalışmaz; adımları ayrı çalıştırın.

-- PuantajRecords.WorkType
ALTER TABLE PuantajRecords ADD WorkType_New int NOT NULL CONSTRAINT DF_PuantajRecords_WorkType_New DEFAULT 1;
GO
UPDATE PuantajRecords SET WorkType_New = CASE
    WHEN WorkType IN (N'Gunduz', N'Gündüz', N'gündüz', N'gunduz', N'1') THEN 1
    WHEN WorkType IN (N'Gece', N'gece', N'2') THEN 2
    WHEN WorkType LIKE N'%Hafta%' OR WorkType = N'3' THEN 3
    WHEN WorkType LIKE N'%Tatil%' OR WorkType LIKE N'%tatil%' OR WorkType LIKE N'%Bayram%' OR WorkType = N'4' THEN 4
    WHEN WorkType IN (N'Izin', N'İzin', N'izin', N'5') THEN 5
    WHEN WorkType IN (N'Rapor', N'rapor', N'6') THEN 6
    ELSE 1 END;
GO
ALTER TABLE PuantajRecords DROP CONSTRAINT DF_PuantajRecords_WorkType_New;
ALTER TABLE PuantajRecords DROP COLUMN WorkType;
EXEC sp_rename 'PuantajRecords.WorkType_New', 'WorkType', 'COLUMN';
GO

-- Her tablo için Unit (ContractItems, MetrajResults, MetrajRuleTemplates)
-- Örnek: ContractItems — diğer tablolarda tablo adını değiştirin.
ALTER TABLE ContractItems ADD Unit_New int NOT NULL CONSTRAINT DF_ContractItems_Unit_New DEFAULT 1;
GO
UPDATE ContractItems SET Unit_New = CASE
    WHEN Unit IN (N'm²', N'm2', N'M2', N'M²', N'metre kare', N'metrekare', N'1') THEN 1
    WHEN Unit IN (N'm³', N'm3', N'M3', N'M³', N'metre küp', N'metrekup', N'2') THEN 2
    WHEN Unit IN (N'm', N'mt', N'metre', N'M', N'3') THEN 3
    WHEN Unit IN (N'kg', N'KG', N'kilogram', N'4') THEN 4
    WHEN Unit IN (N'ton', N'TON', N't', N'5') THEN 5
    WHEN Unit IN (N'adet', N'ADET', N'Adet', N'6') THEN 6
    WHEN Unit IN (N'takım', N'takim', N'Takım', N'TAKIM', N'7') THEN 7
    ELSE 1 END;
GO
ALTER TABLE ContractItems DROP CONSTRAINT DF_ContractItems_Unit_New;
ALTER TABLE ContractItems DROP COLUMN Unit;
EXEC sp_rename 'ContractItems.Unit_New', 'Unit', 'COLUMN';
GO

IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = N'20260623120000_PuantajWorkTypeEnum')
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'20260623120000_PuantajWorkTypeEnum', N'8.0.3');
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = N'20260623130000_MeasurementUnitEnum')
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'20260623130000_MeasurementUnitEnum', N'8.0.3');
GO
