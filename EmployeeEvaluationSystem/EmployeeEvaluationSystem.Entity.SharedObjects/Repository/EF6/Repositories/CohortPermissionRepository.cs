using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    public class CohortPermissionRepository : Repository, ICohortPermissionRepository
    {
        public CohortPermissionRepository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext) : base(unitOfWork, dbcontext)
        {

        }
        public int GetNumberOfCohortPermissions()
        {
            return this.dbcontext.CohortPermissions.Count();
        }

        public IEnumerable<CohortPermission> GetAllCohortPermissions(string userId)
        {
            return this.dbcontext.CohortPermissions;
        }

        public CohortPermission GetCohortPermission(string currentUserId, string cohortPermissionIdToGet)
        {
            return this.dbcontext.CohortPermissions.FirstOrDefault(x => x.ID.Equals(cohortPermissionIdToGet));
        }

        public void DeleteCohortPermission(string currentUserId, string cohortPermissionIdToDelete)
        {
            var cohortPermission = this.GetCohortPermission(currentUserId, cohortPermissionIdToDelete);

            if (cohortPermission == null)
            {
                throw new Exception();
            }

            //TODO : Delete CohortPermission
        }

        public CohortPermission EditCohortPermission(string currentUserId, CohortPermission cohortPermissionToEdit)
        {
            //TODO : Edit CohortPermission

            return null;
        }

        public void AddCohortPermissionToDb(CohortPermission cohortPermissionToAdd)
        {
            this.dbcontext.CohortPermissions.Add(cohortPermissionToAdd);
        }
    }
}
