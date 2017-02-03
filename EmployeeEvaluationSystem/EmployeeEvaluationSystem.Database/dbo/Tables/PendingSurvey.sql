CREATE TABLE [dbo].[PendingSurvey] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [SurveyAvailToMeID] INT              NOT NULL,
    [UserSurveyRoleID]  INT              NOT NULL,
    [DateSent]          DATETIME         NOT NULL,
    [SurveyInstanceID]  INT              NULL,
    [Email]             NVARCHAR (200)   NOT NULL,
    [DateStarted]       DATETIME         NULL,
    [DateCompleted]     DATETIME         NULL,
    [IsDeleted]         BIT              NOT NULL,
    [DateDeleted]       DATETIME         NULL,
    [UserSentById] NVARCHAR(128) NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PendingSurvey_To_Survey] FOREIGN KEY ([SurveyAvailToMeID]) REFERENCES [dbo].[Survey] ([ID]),
    CONSTRAINT [FK_PendingSurvey_To_SurveyInstance] FOREIGN KEY ([SurveyInstanceID]) REFERENCES [dbo].[SurveyInstance] ([ID]),
    CONSTRAINT [FK_PendingSurvey_To_UserSurveyRole] FOREIGN KEY ([UserSurveyRoleID]) REFERENCES [dbo].[UserSurveyRole] ([ID]), 
    CONSTRAINT [FK_PendingSurvey_ToTable] FOREIGN KEY ([UserSentById]) REFERENCES [AspNetUsers]([Id])
);


