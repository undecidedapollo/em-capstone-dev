CREATE TABLE [dbo].[Cohort]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(500) NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [DateDeleted] DATETIME NULL, 
    [DateCreated] DATETIME NOT NULL
)
