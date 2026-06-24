IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'FirmRole'
)
BEGIN
    ALTER TABLE dbo.Users ADD FirmRole int NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'SecondaryFirmRole'
)
BEGIN
    ALTER TABLE dbo.Users ADD SecondaryFirmRole int NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.__EFMigrationsHistory
    WHERE MigrationId = N'20260622120000_AddUserFirmRoles'
)
BEGIN
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES (N'20260622120000_AddUserFirmRoles', N'8.0.0');
END;
