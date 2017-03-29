CREATE TABLE [dbo].[CohortUser] (
    [CohortID]           INT            NOT NULL,
    [UserID]             NVARCHAR (128) NOT NULL,
    [Placeholder] BIT NOT NULL DEFAULT 0, 
    PRIMARY KEY CLUSTERED ([CohortID] , UserID),
    CONSTRAINT [FK_CohortUser_To_Cohort] FOREIGN KEY ([CohortID]) REFERENCES [dbo].[Cohort] ([ID]),
	CONSTRAINT [FK_CohortUser_ToTable] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers]([Id])
);


