Create PROCEDURE GetReportDetails  
as  
BEGIN  
      
    SET NOCOUNT ON;  
  
 Select ID, Name, Description from Cohort
 Select ID, Name from UserSurveyRole
 Select ID, QuestionID, ResponseNum from AnswerInstance 
END
