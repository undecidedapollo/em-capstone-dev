CREATE TABLE [dbo].[Survey] (
    [ID]          INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (500) NOT NULL,
    [CreatedBy]   NVARCHAR (128) NOT NULL,
    [IsDeleted]   BIT            NOT NULL,
    [DateCreated] DATETIME       NOT NULL,
    [DateDeleted] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_Survey_ToTable] FOREIGN KEY ([CreatedBy]) REFERENCES [AspNetUsers]([Id])
);


