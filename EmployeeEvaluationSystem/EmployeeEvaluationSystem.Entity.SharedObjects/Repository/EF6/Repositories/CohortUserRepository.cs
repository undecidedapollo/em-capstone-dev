using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    public class CohortUserRepository : Repository, ICohortUserRepository
    {
        public CohortUserRepository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext) : base(unitOfWork, dbcontext)
        {

        }
        public virtual int GetNumberOfCohortUsers()
        {
            return this.dbcontext.CohortUsers.Count();
        }

        public virtual IEnumerable<CohortUser> GetAllCohortUsers(string userId)
        {
            return this.dbcontext.CohortUsers;
        }

        public virtual CohortUser GetCohortUser(string currentUserId, string cohortUserIdToGet)
        {
            return this.dbcontext.CohortUsers.FirstOrDefault(x => x.UserID.Equals(cohortUserIdToGet));
        }

        public virtual void DeleteCohortUser(string currentUserId, string cohortUserIdToDelete)
        {
            var cohortUser = this.GetCohortUser(currentUserId, cohortUserIdToDelete);

            if (cohortUser == null)
            {
                throw new Exception();
            }
            
            //TODO : Delete CohortUser
        }

        public virtual CohortUser EditCohortUser(string currentUserId, CohortUser cohortUserToEdit)
        {
            //TODO : Edit CohortUser

            return null;
        }

        public virtual void AddCohortUserToDb(string currentUserId, CohortUser cohortUserToAdd)
        {
            this.dbcontext.CohortUsers.Add(cohortUserToAdd);
        }
    }
}
