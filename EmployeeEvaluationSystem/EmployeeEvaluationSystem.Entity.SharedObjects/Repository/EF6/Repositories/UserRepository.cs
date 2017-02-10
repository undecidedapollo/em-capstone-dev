using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;
using System.Data.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    public class UserRepository: Repository, IUserRepository
    {
        public UserRepository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext) : base(unitOfWork, dbcontext)
        {

        }

        public int GetNumberOfUsers()
        {
            return this.dbcontext.AspNetUsers.Count();
        }

        public IEnumerable<AspNetUser> GetAllUsers(string userId)
        {
            return this.dbcontext.AspNetUsers;
        }

        public AspNetUser GetUser(string currentUserId, string userIdToGet)
        {
            return this.dbcontext.AspNetUsers.FirstOrDefault(x => x.Id == userIdToGet);
        }

        public void DeleteUser(string currentUserId, string userIdToGet)
        {
            var user = this.GetUser(currentUserId, userIdToGet);

            if (user == null)
            {
                throw new Exception();
            }

            //TODO Add is deleted.
        }

        public AspNetUser EditUser(string currentUserId, PersonalAspNetUserViewModel userToEdit)
        {
            if (userToEdit == null)
            {
                throw new Exception();
            }

            var user = this.GetUser(currentUserId, userToEdit?.Id);

            if (user == null)
            {
                throw new Exception();
            }

            user.FirstName = userToEdit.FirstName;
            user.LastName = userToEdit.LastName;
            user.PhoneNumber = userToEdit.PhoneNumber;
            user.MailingAddress = userToEdit.MailingAddress;

            return user;
        }
    }
}
