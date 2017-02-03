CREATE TABLE [dbo].[AnswerInstance]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [QuestionID] INT NOT NULL, 
    [ResponseNum] INT NOT NULL, 
    [SurveyInstanceId] INT NOT NULL, 
    CONSTRAINT [FK_AnswerInstance_SurveyInstance] FOREIGN KEY ([SurveyInstanceId]) REFERENCES [SurveyInstance]([ID]), 
    CONSTRAINT [FK_AnswerInstance_To_Question] FOREIGN KEY ([QuestionID]) REFERENCES [Question]([ID])
)
