CREATE PROCEDURE [dbo].[CancelAllOldSurveyLocks]
AS
		update [PendingSurvey]
			Set StatusId = 1, StatusDate = NULL, StatusGuid = NULL
			WHERE [IsDeleted] = 0
			AND [StatusId] = 2
			AND StatusDate < dateadd(minute, -5, getutcdate())
RETURN 0