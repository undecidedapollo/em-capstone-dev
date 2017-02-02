CREATE TABLE [dbo].[QuestionType]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [RatingMax] INT NOT NULL, 
    [RatingMin] INT NOT NULL, 
    [IsRating] BIT NOT NULL, 
    CONSTRAINT [CK_QuestionType_Column] CHECK ([IsRating] = 1) --Need to check when more types get added that only one type is selected or throw an error.
)
