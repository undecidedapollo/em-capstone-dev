using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories
{
    public interface ISurveyRepository : IRepository
    {
        bool CanExistingUserTakeSurvey(string userId, Guid pendingSurveyId); //

        bool CanGuestUserTakeSurvey(string userEmail, Guid pendingSurveyId); //

        PendingSurvey CreatePendingSurveyForExistingUser(string userId, int userRoleId, int surveyAvailableId); //


        PendingSurvey GetPendingSurvey(string userId, Guid pendingSurveyId); //

        ICollection<Survey> GetAllSurveys(string currentUserID); //

        ICollection<PendingSurvey> GetAllSurveysForUser(string userId);

        ICollection<SurveyType> GetAllSurveyTypes(string currentUserID);

        ICollection<PendingSurvey> GetPendingSurveysOfRatersForUser(string userId, Guid pendingSurveyId);

        ICollection<PendingSurvey> GetPendingSurveysOfRatersForUser(string userId, int SurveysAvailableToId);

        ICollection<PendingSurvey> GetPendingSurveysForUser(string userId);

        ICollection<PendingSurvey> GetFinishedSurveysForUser(string userId);

        PendingSurvey GetPendingSurveySYSTEM(Guid pendingSurveyId); //

        ICollection<UserSurveyRole> GetUserSurveyRoles();

        UserSurveyRole GetUserSurveyRole(int roleID); //

        Survey GetSurvey(string userId, int surveyId);

        Survey GetSurveySYSTEM(int surveyId);

        SurveyType GetSurveyType(string userId, int surveyTypeId);

        SurveyType GetSurveyTypeSYSTEM(int surveyTypeId);

        bool DoesSurveyInstanceAlreadyExistSYSTEM(Guid pendingSurveyId);

        SurveyInstance CreateSurveyInstanceForExistingUser(string userIdTakingSurvey, Guid pendingSurveyId);

        SurveyInstance CreateSurveyInstanceForGuestUser(string userIdTakingSurvey, Guid pendingSurveyId);

        SurveyInstance GetPendingSurveyInstance(Guid pendingSurveyId);

        SurveyInstance GetSurveyInstanceById(string userIdTakingSurvey, int surveyInstanceId);

        SurveyInstance GetSurveyInstanceByIdSYSTEM(int surveyInstanceId);

        SurveysAvailable CreateAnAvailableSurveyForCohort(string currentUserID, CreateAvailableSurveyModel model);

        SurveysAvailable GetAnAvailableSurveyForCohort(string currentUserID, int surveyAvailableId, bool track = true);

        ICollection<SurveysAvailable> GetAllOfferedSurveysForCohort(string currentUserID, int cohortId);

        SurveysAvailable GetAnAvailableSurveyForCohortSYSTEM(int surveyAvailableId, bool track = true);

        bool DeleteSurveyAvailable(string userId, int surveyAvailableId);

        bool IsSurveyAvailableStillOpen(int surveyAvailableId);

        AnswerInstance AddAnswerInstanceToSurveyInstance(Guid pendingSurveyId, int questionId, CreateAnswerInstanceModel model);

        AnswerInstance AddAnswerInstanceToSurveyInstance(int surveyInstanceId, int questionId, CreateAnswerInstanceModel model);
        
        Category GetCategory(int categoryId);

        Question GetQuestion(int questionId);

        QuestionType GetQuestionType(int questionTypeId);

        QuestionType GetQuestionTypeOfQuestion(int questionId);

        bool IsQuestionInSurvey(int questionId, int surveyId);

        bool IsQuestionRequired(int questionId);

        LockAndGetSurvey_Result LockAndGetSurvey(Guid pendingSurveyId, Guid? statusGuid = null);

        int CancelSurveyLock(Guid pendingSurveyId);

        int UpdateSurveyLockTime(Guid pendingSurveyId);

        Category GetFirstCategory(int surveyId);

        Category GetNextCategory(int categoryId);

        Category GetPreviousCategory(int categoryId);

        Category GetLastCategory(int surveyId);

        ICollection<Tuple<Question, AnswerInstance>> GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(int categoryId, int surveyInstanceId);

        bool FinishSurvey(int surveyInstanceId, Guid? statusGuid = null);

        SurveyType GetNextAvailableSurveyTypeForSurveyInCohort(int surveyId, int cohortId);

        bool CheckHaveAllSurveysBeenCompleted(int cohortId, int surveyAvailableToId);

        bool CheckHaveAllSelfEvaluationSurveysBeenCompleted(int cohortId, int surveyAvailableToId);

        void TryRemovePendingSurveysSYSTEM(ICollection<PendingSurvey> surveys);

        void TryToAddPendingSurveysSYSTEM(ICollection<PendingSurvey> surveys);

        int CancelAllOldSurveyLocks();

        List<RaterOBJ> GetMostRecentRatersForUser(string userId, int count);

        SurveysAvailable GetPreviousSurveyForCohort(int cohortId, int currentAvailableId);

        void TryMarkAsFinished(int survAvailId);

        void SetDoneEvaluationsToFinished();
    }
}
