CREATE PROCEDURE [dbo].[UpdateLockedSurveyTime]
	@pendingSurveyId uniqueidentifier
AS
	update [PendingSurvey]
			Set StatusDate = GETUTCDATE()
			WHERE [Id] = @pendingSurveyId
			AND [IsDeleted] = 0
			AND [StatusId] = 2
RETURN
