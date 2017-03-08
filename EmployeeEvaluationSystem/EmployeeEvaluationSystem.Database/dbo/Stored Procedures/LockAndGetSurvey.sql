CREATE PROCEDURE [dbo].[LockAndGetSurvey]
	@pendingSurveyId uniqueidentifier,
	@statusGuid uniqueidentifier = null
AS
	DECLARE @newGuid uniqueidentifier;

	SET @newGuid = NEWID()

	IF @statusGuid is null
		BEGIN
			update [PendingSurvey]
			Set StatusId = 2, StatusDate = GETUTCDATE(), StatusGuid = @newGuid
			WHERE [Id] = @pendingSurveyId
			AND [IsDeleted] = 0
			AND [StatusId] = 1
		END
	ELSE
	BEGIN
	
		update [PendingSurvey]
		Set StatusId = 2, StatusDate = GETUTCDATE()
		WHERE [Id] = @pendingSurveyId
		AND [IsDeleted] = 0
		AND [StatusId] = 2
		AND [StatusGuid] = @statusGuid;

		SET @newGuid = @statusGuid;
	END

	SELECT TOP 1 * FROM [PendingSurvey]
	WHERE [Id] = @pendingSurveyId
	AND [StatusId] = 2
	AND [IsDeleted] = 0
	AND [StatusGuid] = @newGuid
RETURN