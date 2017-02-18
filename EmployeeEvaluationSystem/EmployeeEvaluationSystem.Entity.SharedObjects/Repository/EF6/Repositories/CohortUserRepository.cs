using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    class CohortUserRepository : Repository, ICohortUserRepository
    {
        public CohortUserRepository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext) : base(unitOfWork, dbcontext)
        {

        }
        public int GetNumberOfCohortUsers()
        {
            return this.dbcontext.CohortUsers.Count();
        }

        public IEnumerable<CohortUser> GetAllCohortUsers(string userId)
        {
            return this.dbcontext.CohortUsers;
        }

        public CohortUser GetCohortUser(string currentUserId, string cohortUserIdToGet)
        {
            return this.dbcontext.CohortUsers.FirstOrDefault(x => x.UserID.Equals(cohortUserIdToGet));
        }

        public void DeleteCohortUser(string currentUserId, string cohortUserIdToDelete)
        {
            var cohortUser = this.GetCohortUser(currentUserId, cohortUserIdToDelete);

            if (cohortUser == null)
            {
                throw new Exception();
            }
            
            //TODO : Delete CohortUser
        }

        public CohortUser EditCohortUser(string currentUserId, CohortUser cohortUserToEdit)
        {
            //TODO : Edit CohortUser

            return null;
        }
    }
}
