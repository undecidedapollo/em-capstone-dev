using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories
{
    public interface ICohortRepository : IRepository
    {
        int GetNumberOfCohorts();

        IEnumerable<Cohort> GetAllCohorts(string userId);
        Cohort GetCohort(string currentUserId, int? cohortIdToGet);
        bool DeleteCohort(string currentUserId, int cohortIdToGet);
        Cohort EditCohort(string currentUserId, Cohort cohortToEdit);
        void AddCohortToDb(string currentUserId, Cohort cohortToAdd);
        IEnumerable<AspNetUser> GetAllUsersThatAreNotPartOfACohort(string userId);
    }
}
