CREATE TABLE [dbo].[Question] (
    [ID]             INT            IDENTITY (1, 1) NOT NULL,
    [CategoryID]     INT            NOT NULL,
    [QuestionTypeId] INT            NOT NULL,
    [IsRequired]     BIT            NOT NULL,
    [IsDeleted]      BIT            NOT NULL,
    [DateCreated]    DATETIME       NOT NULL,
    [DateDeleted]    DATETIME       NULL,
    [Name]           NVARCHAR (50)  NOT NULL,
    [DisplayText]    NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Question_ToTable] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category] ([ID]),
    CONSTRAINT [FK_Question_ToTable_1] FOREIGN KEY ([QuestionTypeId]) REFERENCES [dbo].[QuestionType] ([ID])
);


