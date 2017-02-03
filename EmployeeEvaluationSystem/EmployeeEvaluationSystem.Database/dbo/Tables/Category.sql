CREATE TABLE [dbo].[Category]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [SurveyID] INT NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [Description] NVARCHAR(500) NOT NULL, 
    [DateCreated] DATETIME NULL, 
    CONSTRAINT [FK_Category_ToTable] FOREIGN KEY ([SurveyID]) REFERENCES [Survey]([ID])
)
