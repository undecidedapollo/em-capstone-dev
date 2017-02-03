CREATE TABLE [dbo].[SiteWidePermission]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Cohort] INT NOT NULL, 
    [User] INT NOT NULL, 
    [Survey] INT NOT NULL, 
	CONSTRAINT [FK_SiteWidePermission_To_Perm1] FOREIGN KEY ([Cohort]) REFERENCES [Permission]([ID]),
	CONSTRAINT [FK_SiteWidePermission_To_Perm2] FOREIGN KEY ([User]) REFERENCES [Permission]([ID]),
	CONSTRAINT [FK_SiteWidePermission_To_Perm3] FOREIGN KEY ([Survey]) REFERENCES [Permission]([ID]),
)
