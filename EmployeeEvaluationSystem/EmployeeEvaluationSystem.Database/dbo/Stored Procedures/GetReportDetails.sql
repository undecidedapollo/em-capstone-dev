Create PROCEDURE GetReportDetails  
as  
BEGIN  
      
    SET NOCOUNT ON;  
  
 Select UserName from AspNetUsers  
 Select UserId,RoleId from AspNetUserRoles
 Select ID, Name from Cohort
 Select CohortID, UserID from CohortUser
END
