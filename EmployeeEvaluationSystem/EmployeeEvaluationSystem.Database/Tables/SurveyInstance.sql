CREATE TABLE [dbo].[SurveyInstance]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SurveyID] INT NOT NULL, 
    [UserTakenBy] NVARCHAR(128) NULL, 
    [DateStarted] DATETIME NOT NULL, 
    [DateFinished] DATETIME NULL, 
    [SurveyTypeId] INT NOT NULL, 
    [UserSurveyRoleId] INT NOT NULL, 
    CONSTRAINT [FK_SurveyInstance_To_Survey] FOREIGN KEY ([SurveyID]) REFERENCES [Survey]([ID]), 
    CONSTRAINT [FK_SurveyInstance_To_SurveyType] FOREIGN KEY ([SurveyTypeId]) REFERENCES [SurveyType]([ID]), 
    CONSTRAINT [FK_SurveyInstance_To_UserSurveyRole] FOREIGN KEY ([UserSurveyRoleId]) REFERENCES [UserSurveyRole]([Id])
)
