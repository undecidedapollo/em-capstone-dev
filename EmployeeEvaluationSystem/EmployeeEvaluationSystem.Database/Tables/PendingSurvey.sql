CREATE TABLE [dbo].[PendingSurvey]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [SurveyAvailToMeID] INT NOT NULL, 
    [UserSurveyRoleID] INT NOT NULL, 
    [DateSent] DATETIME NOT NULL, 
    [SurveyInstanceID] INT NULL, 
    [Email] NVARCHAR(200) NOT NULL, 
    [DateStarted] DATETIME NULL, 
    [DateCompleted] DATETIME NULL, 
    [IsDeleted] BIT NOT NULL, 
    [DateDeleted] DATETIME NULL, 
    CONSTRAINT [FK_PendingSurvey_To_Survey] FOREIGN KEY ([SurveyAvailToMeID]) REFERENCES [Survey]([ID]), 
    CONSTRAINT [FK_PendingSurvey_To_UserSurveyRole] FOREIGN KEY ([UserSurveyRoleID]) REFERENCES [UserSurveyRole]([ID]), 
    CONSTRAINT [FK_PendingSurvey_To_SurveyInstance] FOREIGN KEY ([SurveyInstanceID]) REFERENCES [SurveyInstance]([ID])
)
