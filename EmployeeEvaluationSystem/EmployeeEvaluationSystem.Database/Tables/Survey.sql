CREATE TABLE [dbo].[Survey]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(500) NOT NULL, 
    [CreatedBy] NVARCHAR(128) NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateDeleted] DATETIME NULL
)
