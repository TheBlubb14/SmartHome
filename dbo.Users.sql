USE [SmartHomeDatabase]
GO

/****** Objekt: Table [dbo].[Users] Skriptdatum: 05.07.2016 13:24:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users] (
    [UserID]      NVARCHAR (255) NOT NULL,
    [FirstName]   NVARCHAR (MAX) NULL,
    [LastName]    NVARCHAR (MAX) NULL,
    [FirstJoined] NVARCHAR (MAX) NULL,
    [Group]       INT            NOT NULL
);


