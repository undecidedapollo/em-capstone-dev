CREATE PROCEDURE [dbo].[CancelSurveyLock]
	@pendingSurveyId uniqueidentifier
AS
	update [PendingSurvey]
			Set StatusId = 1, StatusDate = NULL, StatusGuid = NULL
			WHERE [Id] = @pendingSurveyId
			AND [IsDeleted] = 0
			AND [StatusId] = 2
RETURN
