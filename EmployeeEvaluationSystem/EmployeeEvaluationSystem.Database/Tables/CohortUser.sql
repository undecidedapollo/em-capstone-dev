CREATE TABLE [dbo].[CohortUser]
(
	[CohortID] INT NOT NULL PRIMARY KEY, 
    [UserID] NVARCHAR(128) NOT NULL, 
    [CohortPermissionId] INT NOT NULL, 
    CONSTRAINT [FK_CohortUser_To_Cohort] FOREIGN KEY ([CohortID]) REFERENCES [Cohort]([ID]), 
    CONSTRAINT [FK_CohortUser_To_Permissions] FOREIGN KEY ([CohortPermissionId]) REFERENCES [CohortPermission]([Id])
)
