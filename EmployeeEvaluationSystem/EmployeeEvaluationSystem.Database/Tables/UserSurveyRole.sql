CREATE TABLE [dbo].[UserSurveyRole]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateDeleted] DATETIME NULL
)
