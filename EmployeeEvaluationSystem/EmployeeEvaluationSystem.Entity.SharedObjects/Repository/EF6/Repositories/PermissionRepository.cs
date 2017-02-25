using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    public class PermissionRepository : Repository, IPermissionRepository
    {
        public PermissionRepository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext) : base(unitOfWork, dbcontext)
        {

        }
        public int GetNumberOfPermissions()
        {
            return this.dbcontext.Permissions.Count();
        }

        public IEnumerable<Permission> GetAllPermissions(string userId)
        {
            return this.dbcontext.Permissions;
        }

        public Permission GetPermission(string currentUserId, int? permissionIdToGet)
        {
            var permission = this.dbcontext.Permissions.FirstOrDefault(x => x.ID == permissionIdToGet);

            return permission;
        }

        public void DeletePermission(string currentUserId, int permissionIdToDelete)
        {
            var permission = this.GetPermission(currentUserId, permissionIdToDelete);

            this.dbcontext.Permissions.Remove(permission);
        }

        public void EditPermission(string currentUserId, Permission permissionToEdit)
        {
            this.dbcontext.Entry(permissionToEdit).State = EntityState.Modified;
        }

        public void AddPermissionToDb(string currentUserId, Permission permissionToAdd)
        {
            this.dbcontext.Permissions.Add(permissionToAdd);
        }
    }
}
