USE [SmartHomeDatabase]
GO

/****** Objekt: Table [dbo].[Objects] Skriptdatum: 05.07.2016 13:23:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Objects] (
    [ObjectID] INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (MAX) NOT NULL,
    [Status]   NVARCHAR (MAX) NULL
);


