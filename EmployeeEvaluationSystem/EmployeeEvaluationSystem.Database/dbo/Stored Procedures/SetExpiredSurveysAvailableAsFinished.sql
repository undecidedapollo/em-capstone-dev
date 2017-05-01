CREATE PROCEDURE [dbo].[SetExpiredSurveysAvailableAsFinished]
AS
	Update [SurveysAvailable]
	SET DateCompleted = GETUTCDATE(), IsCompleted = 1
	WHERE DateClosed < GETUTCDATE()
	AND IsCompleted = 0
	AND IsDeleted = 0
RETURN
