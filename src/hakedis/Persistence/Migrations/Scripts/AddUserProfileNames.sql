-- AddUserProfileNames migration (manuel çalıştırma)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'FirstName'
)
BEGIN
    ALTER TABLE dbo.Users ADD FirstName nvarchar(100) NOT NULL CONSTRAINT DF_Users_FirstName DEFAULT '';
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'LastName'
)
BEGIN
    ALTER TABLE dbo.Users ADD LastName nvarchar(100) NOT NULL CONSTRAINT DF_Users_LastName DEFAULT '';
END;

IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624220000_AddUserProfileNames'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624220000_AddUserProfileNames', N'8.0.3');
END;
