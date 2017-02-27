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

        Survey GetSurvey(string userId, int surveyId);

        Survey GetSurveySYSTEM(int surveyId);

        SurveyType GetSurveyType(string userId, int surveyTypeId);

        SurveyType GetSurveyTypeSYSTEM(int surveyTypeId);

        PendingSurvey GetPendingSurvey(string userId, Guid pendingSurveyId);
        PendingSurvey GetPendingSurveySYSTEM(Guid pendingSurveyId);

        bool DoesSurveyInstanceAlreadyExistSYSTEM(Guid pendingSurveyId);

        SurveyInstance CreateSurveyInstanceForExistingUser(string userIdTakingSurvey, Guid pendingSurveyId);

        SurveyInstance CreateSurveyInstanceForGuestUser(string userIdTakingSurvey, Guid pendingSurveyId);

        SurveyInstance GetPendingSurveyInstance(Guid pendingSurveyId);

        SurveysAvailable CreateAnAvailableSurveyForCohort(string currentUserID, CreateAvailableSurveyModel model);

        SurveysAvailable GetAnAvailableSurveyForCohort(string currentUserID, int surveyAvailableId);

        SurveysAvailable GetAnAvailableSurveyForCohortSYSTEM(int surveyAvailableId);

        SurveysAvailable DeleteSurveyAvailable(string userId, int surveyAvailableId);

        bool IsSurveyAvailableStillOpen(int surveyAvailableId);
    }
}
