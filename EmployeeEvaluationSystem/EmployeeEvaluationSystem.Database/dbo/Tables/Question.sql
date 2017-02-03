CREATE TABLE [dbo].[Question]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CategoryID] INT NOT NULL, 
    [QuestionTypeId] INT NOT NULL, 
    [IsRequired] BIT NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateDeleted] DATETIME NULL, 
    CONSTRAINT [FK_Question_ToTable] FOREIGN KEY ([CategoryID]) REFERENCES [Category]([ID]), 
    CONSTRAINT [FK_Question_ToTable_1] FOREIGN KEY ([QuestionTypeId]) REFERENCES [QuestionType]([ID])
)
