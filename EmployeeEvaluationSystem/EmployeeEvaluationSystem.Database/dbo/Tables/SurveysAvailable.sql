CREATE TABLE [dbo].[SurveysAvailable]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CohortID] INT NOT NULL, 
    [SurveyID] INT NOT NULL, 
    [DateOpen] DATETIME NOT NULL, 
    [DateClosed] DATETIME NOT NULL, 
	[SurveyTypeId] INT NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateDeleted] DATETIME NULL, 
    CONSTRAINT [FK_SurveysAvailable_To_Cohort] FOREIGN KEY ([CohortID]) REFERENCES [Cohort]([ID]),
	CONSTRAINT [FK_SurveysAvailable_To_Survey] FOREIGN KEY ([SurveyID]) REFERENCES [Survey]([ID]),
	CONSTRAINT [FK_SurveysAvailable_To_SurveyType] FOREIGN KEY ([SurveyTypeId]) REFERENCES [SurveyType]([ID])
)
