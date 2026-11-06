/*
  Mark InitialState migration as already applied WITHOUT running its Up() script.

  USE ONLY WHEN:
  - Tables from migration 20261106172938_InitialState already exist and match the model.
  - You got: "There is already an object named 'Categories'" on dotnet ef database update
    because __EFMigrationsHistory was empty or out of sync.

  If the database is only partially created (e.g. Categories/Products but no AspNetUsers),
  do NOT run this — drop the DB and use dotnet ef database update on a clean database.

  Change USE below to your database name if needed.
*/

-- USE [TechShopDb];
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId])
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261106172938_InitialState'
)
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261106172938_InitialState', N'10.0.14');
END
GO
