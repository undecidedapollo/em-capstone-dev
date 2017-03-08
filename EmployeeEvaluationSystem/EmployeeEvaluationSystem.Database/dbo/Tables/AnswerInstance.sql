CREATE TABLE [dbo].[AnswerInstance] (
    [ID]               INT IDENTITY (1, 1) NOT NULL,
    [QuestionID]       INT NOT NULL,
    [ResponseNum]      INT NOT NULL,
    [SurveyInstanceId] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_AnswerInstance_SurveyInstance] FOREIGN KEY ([SurveyInstanceId]) REFERENCES [dbo].[SurveyInstance] ([ID]),
    CONSTRAINT [FK_AnswerInstance_To_Question] FOREIGN KEY ([QuestionID]) REFERENCES [dbo].[Question] ([ID]),
    CONSTRAINT [AK_AnswerInstance_Column] UNIQUE NONCLUSTERED ([SurveyInstanceId] ASC, [QuestionID] ASC)
);


