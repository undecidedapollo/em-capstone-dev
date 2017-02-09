CREATE TABLE [dbo].[LocationAddress]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Address] NVARCHAR(128) NOT NULL, 
    [Address2] NVARCHAR(128) NULL, 
    [City] NVARCHAR(128) NOT NULL, 
    [State] NVARCHAR(128) NOT NULL, 
    [ZipCode] NVARCHAR(128) NOT NULL, 
    [DateCreated] DATETIME NOT NULL
)
