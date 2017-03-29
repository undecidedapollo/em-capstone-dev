using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories
{
    public interface IUserRepository : IRepository
    {
        int GetNumberOfUsers();

        IEnumerable<AspNetUser> GetAllUsers(string userId);
        AspNetUser GetUser(string currentUserId, string userIdToGet);
        void DeleteUser(string currentUserId, string userIdToGet);

        AspNetUser EditUser(string currentUserId, PersonalAspNetUserViewModel userToEdit);
        bool isUserAdmin(string userId);


    }
}
