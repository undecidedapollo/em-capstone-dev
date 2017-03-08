﻿CREATE TABLE [dbo].[PendingSurvey] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [SurveyAvailToMeID] INT              NOT NULL,
    [UserSurveyRoleID]  INT              NOT NULL,
    [DateSent]          DATETIME         NOT NULL,
    [SurveyInstanceID]  INT              NULL,
    [Email]             NVARCHAR (200)   NULL,
    [IsDeleted]         BIT              NOT NULL,
    [DateDeleted]       DATETIME         NULL,
    [UserSentById]      NVARCHAR (128)   NOT NULL,
    [UserForId]         NVARCHAR (128)   NULL,
    [StatusId]          INT              DEFAULT ((1)) NOT NULL,
    [StatusGuid]        UNIQUEIDENTIFIER NULL,
    [StatusDate]        DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_PendingSurvey_UserExistsOrEmailIsNotNull] CHECK ([UserForId] IS NULL AND [Email] IS NOT NULL OR [UserForId] IS NOT NULL AND [Email] IS NULL),
    CONSTRAINT [FK_PendingSurvey_To_Survey] FOREIGN KEY ([SurveyAvailToMeID]) REFERENCES [dbo].[SurveysAvailable] ([ID]),
    CONSTRAINT [FK_PendingSurvey_To_SurveyInstance] FOREIGN KEY ([SurveyInstanceID]) REFERENCES [dbo].[SurveyInstance] ([ID]),
    CONSTRAINT [FK_PendingSurvey_To_UserSurveyRole] FOREIGN KEY ([UserSurveyRoleID]) REFERENCES [dbo].[UserSurveyRole] ([ID]),
    CONSTRAINT [FK_PendingSurvey_ToTable] FOREIGN KEY ([UserSentById]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_PendingSurvey_ToTable_1] FOREIGN KEY ([UserForId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_PendingSurvey_ToTable_2] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[Status] ([Id])
);




