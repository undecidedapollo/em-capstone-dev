using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.SharedObjects.Helpers.Locks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Helpers.Locks
{
    public class SurveyLockManager : IDisposable
    {
        private LockManager<LockAndGetSurvey_Result> lockManager;
        private IUnitOfWork unitOfWork;

        public SurveyLockManager(Guid pendingSurveyId)
        {
            unitOfWork = new UnitOfWork();


            this.lockManager = new LockManager<LockAndGetSurvey_Result>(() =>
            {
                return unitOfWork.Surveys.LockAndGetSurvey(pendingSurveyId);
            }, () => unitOfWork.Surveys.CancelSurveyLock(pendingSurveyId));

            this.lockManager.Before();
        }

        public SurveyLockManager(Guid pendingSurveyId, Guid statusGuid)
        {
            unitOfWork = new UnitOfWork();


            this.lockManager = new LockManager<LockAndGetSurvey_Result>(() =>
            {
                return unitOfWork.Surveys.LockAndGetSurvey(pendingSurveyId, statusGuid);
            }, () => unitOfWork.Surveys.CancelSurveyLock(pendingSurveyId));

            this.lockManager.Before();
        }

        public LockAndGetSurvey_Result BeforeValue()
        {
            return this.lockManager.BeforeValue();
        }

        public void Dispose()
        {
            this.lockManager.After();
        }
    }
}
