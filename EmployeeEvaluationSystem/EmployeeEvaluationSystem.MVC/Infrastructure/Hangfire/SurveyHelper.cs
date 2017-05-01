using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Infrastructure.Hangfire
{
    public static class SurveyHelper
    {

        public static void CheckAndMarkSurveyFinished(int cohortId, int surveysAvailableID)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var result = unitOfWork.Surveys.CheckHaveAllSelfEvaluationSurveysBeenCompleted(cohortId, surveysAvailableID);

                if (result)
                {
                    unitOfWork.Complete();
                }
            }
        }

        public static void CancelOldSurveyLocks()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.Surveys.CancelAllOldSurveyLocks();
            }
        }


        public static void SetExpiredSurveysToCompleted()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.Surveys.SetDoneEvaluationsToFinished();
            }
        }
    }
}