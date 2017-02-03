CREATE TABLE [dbo].[SurveysAvailableTo]
(
	[SurveyAvailableID] INT NOT NULL PRIMARY KEY, 
    [UserSurveyRoleId] INT NOT NULL, 
    [Quantity] INT NOT NULL, 
    CONSTRAINT [FK_SurveysAvailableTo_SurveysAvailable] FOREIGN KEY ([SurveyAvailableID]) REFERENCES [SurveysAvailable]([ID]), 
	    CONSTRAINT [FK_SurveysAvailableTo_UserTakenRole] FOREIGN KEY ([UserSurveyRoleId]) REFERENCES [UserSurveyRole]([ID]), 
    CONSTRAINT [CK_SurveysAvailableTo_Column] CHECK (Quantity > 0)
)
