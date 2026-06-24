-- Run this if Update-Database still fails or admin user was deleted by the partial migration.
-- Safe to run multiple times.

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'FirmRole'
)
BEGIN
    ALTER TABLE dbo.Users ADD FirmRole int NULL;
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'SecondaryFirmRole'
)
BEGIN
    ALTER TABLE dbo.Users ADD SecondaryFirmRole int NULL;
END;

DELETE FROM dbo.UserOperationClaims
WHERE Id IN (
    'a455f44d-e87c-46f8-8187-35df6f6ff130',
    'eac3afcd-86e6-4751-aa8d-608fce4444be'
)
   OR UserId IN (
    '5bba1952-5044-4c8c-89c3-59527756b2e0',
    'dfc3b388-1c0e-4c92-bb77-759140f27e11'
);

DELETE FROM dbo.Users
WHERE Id IN (
    '5bba1952-5044-4c8c-89c3-59527756b2e0',
    'dfc3b388-1c0e-4c92-bb77-759140f27e11'
);

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'narch@kodlama.io')
BEGIN
    INSERT INTO dbo.Users (
        Id, AuthenticatorType, CreatedDate, DeletedDate, Email,
        FirmRole, PasswordHash, PasswordSalt, SecondaryFirmRole, TenantId, UpdatedDate
    )
    VALUES (
        'dfc3b388-1c0e-4c92-bb77-759140f27e11',
        0,
        '0001-01-01T00:00:00.0000000',
        NULL,
        N'narch@kodlama.io',
        NULL,
        0x73765E55AE2193AF5B6088CBB58CFF34DFC39163A032CBB6AFBEC6D171CCE7D5491ADD749496216FE859E02B2E9D9DAA19EA818B692DC7B710C3E58E58E89DEA,
        0x94A8164555AEB9D4510DBDDFE3C22E4B5BFC51B9BE4BC2A2DFB3FF63ECF61A63318E770A1B754A857C0796435F100ED960A8B1BB4997675DAF5BE2A3240988ADB9332EB3820A1AC09C8B99BD97860819D9386F765BF2072D709F0EF65FB03E5A2CEBC03532BF279AF6A2ECD5E0EF7BD7B73A0F64A6ECC59C2F602987B164D7A5,
        NULL,
        NULL,
        NULL
    );
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.UserOperationClaims
    WHERE UserId = 'dfc3b388-1c0e-4c92-bb77-759140f27e11' AND OperationClaimId = 1
)
BEGIN
    INSERT INTO dbo.UserOperationClaims (
        Id, CreatedDate, DeletedDate, OperationClaimId, UpdatedDate, UserId
    )
    VALUES (
        'eac3afcd-86e6-4751-aa8d-608fce4444be',
        '0001-01-01T00:00:00.0000000',
        NULL,
        1,
        NULL,
        'dfc3b388-1c0e-4c92-bb77-759140f27e11'
    );
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.__EFMigrationsHistory
    WHERE MigrationId = N'20260624211301_AddRole'
)
BEGIN
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES (N'20260624211301_AddRole', N'8.0.3');
END;
