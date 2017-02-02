CREATE TABLE [dbo].[SurveyType]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [DeletedDate] DATETIME NULL
)
