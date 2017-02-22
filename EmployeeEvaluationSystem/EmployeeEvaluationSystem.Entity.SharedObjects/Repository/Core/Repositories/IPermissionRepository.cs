using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories
{
    public interface IPermissionRepository : IRepository
    {
        int GetNumberOfPermissions();
        IEnumerable<Permission> GetAllPermissions(string userId);
        Permission GetPermission(string currentUserId, int? permissionIdToGet);
        void DeletePermission(string currentUserId, int permissionIdToDelete);
        void EditPermission(string currentUserId, Permission permissionToEdit);
        void AddPermissionToDb(string currentUserId, Permission permission);
    }
}
