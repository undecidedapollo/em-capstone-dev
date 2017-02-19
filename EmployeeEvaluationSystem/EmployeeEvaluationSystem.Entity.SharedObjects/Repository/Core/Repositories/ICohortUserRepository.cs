using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories
{
    public interface ICohortUserRepository : IRepository
    {
        int GetNumberOfCohortUsers();
        IEnumerable<CohortUser> GetAllCohortUsers(string userId);
        CohortUser GetCohortUser(string currentUserId, string cohortUserIdToGet);
        void DeleteCohortUser(string currentUserId, string cohortUserIdToDelete);
        CohortUser EditCohortUser(string currentUserId, CohortUser cohortUserToEdit);
        void AddCohortUserToDb(CohortUser cohortUser);
    }
}
