USE [SmartHomeDatabase]
GO

/****** Objekt: Table [dbo].[Status] Skriptdatum: 05.07.2016 13:23:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Status] (
    [StatusID] INT            IDENTITY (1, 1) NOT NULL,
    [UserID]   NVARCHAR (255) NOT NULL,
    [Status]   NVARCHAR (MAX) NULL,
    [Perma]    BIT            NOT NULL
);


