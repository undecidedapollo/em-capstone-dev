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
        bool CanExistingUserTakeSurvey(string userId, Guid pendingSurveyId);
        bool CanGuestUserTakeSurvey(string userEmail, Guid pendingSurveyId);

        PendingSurvey CreatePendingSurveyForExistingUser(string userId, int userRoleId, int surveyAvailableId);

        PendingSurvey CreatePendingSurveyForGuestUser(string userEmail, int userRoleId, int surveyAvailableId);

        PendingSurvey GetPendingSurvey(string userId, Guid pendingSurveyId);

        ICollection<PendingSurvey> GetAllSurveysForUser(string userId);

        ICollection<PendingSurvey> GetPendingSurveysForUser(string userId);

        ICollection<PendingSurvey> GetFinishedSurveysForUser(string userId);

        PendingSurvey GetPendingSurveySYSTEM(Guid pendingSurveyId);

        UserSurveyRole GetUserSurveyRole(int roleID);

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

        SurveysAvailable GetAnAvailableSurveyForCohort(string currentUserID, int surveyAvailableId);

        SurveysAvailable GetAnAvailableSurveyForCohortSYSTEM(int surveyAvailableId);

        SurveysAvailable DeleteSurveyAvailable(string userId, int surveyAvailableId);

        bool IsSurveyAvailableStillOpen(int surveyAvailableId);

        AnswerInstance AddAnswerInstanceToSurveyInstance(Guid pendingSurveyId, int questionId, CreateAnswerInstanceModel model);

        AnswerInstance AddAnswerInstanceToSurveyInstance(int surveyInstanceId, int questionId, CreateAnswerInstanceModel model);

        
        Category GetCategory(int categoryId);

        Question GetQuestion(int questionId);

        QuestionType GetQuestionType(int questionTypeId);

        QuestionType GetQuestionTypeOfQuestion(int questionId);

        bool IsQuestionInSurvey(int questionId, int surveyId);

        LockAndGetSurvey_Result LockAndGetSurvey(Guid pendingSurveyId, Guid? statusGuid = null);

        int CancelSurveyLock(Guid pendingSurveyId);

        int UpdateSurveyLockTime(Guid pendingSurveyId);

        Category GetFirstCategory(int surveyId);

        Category GetNextCategory(int categoryId);

        Category GetPreviousCategory(int categoryId);

        Category GetLastCategory(int surveyId);

        ICollection<Tuple<Question, AnswerInstance>> GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(int categoryId, int surveyInstanceId);


    }
}
