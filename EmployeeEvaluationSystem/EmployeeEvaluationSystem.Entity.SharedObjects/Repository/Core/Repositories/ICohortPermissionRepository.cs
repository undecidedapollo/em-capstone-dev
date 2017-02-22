using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories
{
    public interface ICohortPermissionRepository : IRepository
    {
        int GetNumberOfCohortPermissions();
        IEnumerable<CohortPermission> GetAllCohortPermissions(string userId);
        CohortPermission GetCohortPermission(string currentUserId, int? cohortPermissionIdToGet);
        void DeleteCohortPermission(string currentUserId, int cohortPermissionIdToDelete);
        void EditCohortPermission(string currentUserId, CohortPermission cohortPermissionToEdit);
        void AddCohortPermissionToDb(string currentUserId, CohortPermission cohortPermission);
    }
}
