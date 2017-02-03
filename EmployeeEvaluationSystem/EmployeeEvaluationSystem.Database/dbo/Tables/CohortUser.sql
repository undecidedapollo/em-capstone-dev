CREATE TABLE [dbo].[CohortUser] (
    [CohortID]           INT            NOT NULL,
    [UserID]             NVARCHAR (128) NOT NULL,
    [CohortPermissionId] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([CohortID] ASC),
    CONSTRAINT [FK_CohortUser_To_Cohort] FOREIGN KEY ([CohortID]) REFERENCES [dbo].[Cohort] ([ID]),
    CONSTRAINT [FK_CohortUser_To_Permissions] FOREIGN KEY ([CohortPermissionId]) REFERENCES [dbo].[CohortPermission] ([ID]),
	CONSTRAINT [FK_CohortUser_ToTable] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers]([Id])
);


