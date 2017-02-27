using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;
using EmployeeEvaluationSystem.SharedObjects.Exceptions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    public class SurveyRepository : Repository, ISurveyRepository
    {
        public SurveyRepository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext) : base(unitOfWork, dbcontext)
        {

        }

        public bool CanExistingUserTakeSurvey(string userId, Guid pendingSurveyId)
        {
            var survey = this.GetPendingSurveySYSTEM(pendingSurveyId);

            if(survey == null)
            {
                return false;
            }

            if(survey.UserForId == null)
            {
                return false;
            }

            if(survey.UserForId != userId)
            {
                return false;
            }

            return true;
        }

        public bool CanNewUserTakeSurvey(string userEmail, Guid pendingSurveyId)
        {
            var survey = this.GetPendingSurveySYSTEM(pendingSurveyId);

            if (survey == null)
            {
                return false;
            }

            if (survey.Email == null)
            {
                return false;
            }

            if (survey.Email != userEmail)
            {
                return false;
            }

            return true;
        }

        public PendingSurvey GetPendingSurvey(string userId, Guid pendingSurveyId)
        {
            return this.GetPendingSurveySYSTEM(pendingSurveyId);
        }

        public PendingSurvey GetPendingSurveySYSTEM(Guid pendingSurveyId)
        {
            return this.dbcontext.PendingSurveys.FirstOrDefault(x => x.Id == pendingSurveyId);
        }
    }
}
