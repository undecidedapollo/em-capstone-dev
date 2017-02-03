CREATE TABLE [dbo].[Permission]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Add] BIT NOT NULL, 
    [Edit] BIT NOT NULL, 
    [Delete] BIT NOT NULL, 
    [View] BIT NOT NULL
)
