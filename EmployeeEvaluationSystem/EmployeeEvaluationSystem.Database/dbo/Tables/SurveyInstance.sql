CREATE TABLE [dbo].[SurveyInstance] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [SurveyID]         INT            NOT NULL,
    [UserTakenBy]      NVARCHAR (128) NULL,
    [DateStarted]      DATETIME       NOT NULL,
    [DateFinished]     DATETIME       NULL
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SurveyInstance_To_Survey] FOREIGN KEY ([SurveyID]) REFERENCES [dbo].[Survey] ([ID]),
	CONSTRAINT [FK_SurveyInstance_ToTable] FOREIGN KEY ([UserTakenBy]) REFERENCES [AspNetUsers]([Id])
);


