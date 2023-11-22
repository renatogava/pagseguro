

CREATE TABLE [dbo].[Log] (
    [Id] int NOT NULL IDENTITY,
    [Message] nvarchar(max) NULL,
    [MessageTemplate] nvarchar(max) NULL,
    [LevelId] nvarchar(max) NULL,
    [TimeStamp] datetime2 NOT NULL,
    [Exception] nvarchar(max) NULL,
    [Properties] nvarchar(max) NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [dbo].[Setting] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Value] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Setting] PRIMARY KEY ([Id])
);
GO
