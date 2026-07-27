IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Todo] (
    [ID] int NOT NULL IDENTITY,
    [Description] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Todo] PRIMARY KEY ([ID])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240621154946_InitialCreate', N'8.0.6');
GO

COMMIT;
GO

