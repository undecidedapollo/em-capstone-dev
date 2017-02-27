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
        bool CanNewUserTakeSurvey(string userEmail, Guid pendingSurveyId);
        PendingSurvey GetPendingSurvey(string userId, Guid pendingSurveyId);
        PendingSurvey GetPendingSurveySYSTEM(Guid pendingSurveyId);

        bool DoesSurveyInstanceAlreadyExistSYSTEM(Guid pendingSurveyId);

        SurveyInstance CreateSurveyInstance(string userIdTakingSurvey, Guid pendingSurveyId);
        SurveyInstance GetPendingSurveyInstance(Guid pendingSurveyId);



    }
}
