using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    class CohortRepository : Repository, ICohortRepository
    {
        public CohortRepository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext) : base(unitOfWork, dbcontext)
        {

        }

        public int GetNumberOfCohorts()
        {
            return this.dbcontext.Cohorts.Count();
        }

        public IEnumerable<Cohort> GetAllCohorts(string userId)
        {
            return this.dbcontext.Cohorts;
        }

        public Cohort GetCohort(string currentUserId, int? cohortIdToGet)
        {
            return this.dbcontext.Cohorts.FirstOrDefault(x => x.ID == cohortIdToGet);
        }

        public void DeleteCohort(string currentUserId, int cohortIdToGet)
        {
            var cohort = this.GetCohort(currentUserId, cohortIdToGet);

            if (cohort == null)
            {
                throw new Exception();
            }

            cohort.IsDeleted = true;
        }

        public Cohort EditCohort(string currentUserId, Cohort cohortToEdit)
        {
            if (cohortToEdit == null)
            {
                throw new Exception();
            }

            var cohort = this.GetCohort(currentUserId, cohortToEdit.ID);

            if (cohort == null)
            {
                throw new Exception();
            }

            cohort.Name = cohortToEdit.Name;
            cohort.Description = cohortToEdit.Description;

            return cohort;
        }

        public IEnumerable<AspNetUser> GetAllUsersThatAreNotPartOfACohort(string userId)
        {
            
            var users = unitOfWork.Users.GetAllUsers(userId).ToList();
            

            foreach (var user in users)
            {
                if (this.dbcontext.CohortUsers.Any(x => x.UserID.Equals(user.Id)))
                {
                    users.Remove(user);
                }
            }

            return users;
        }

        public Cohort CreateCohort(int id, string name, string description, DateTime dateCreated)
        {
            var cohort = new Cohort
            {
                ID = id,
                Name = name,
                Description = description,
                DateCreated = dateCreated,
                IsDeleted = false,
                DateDeleted = null
            };

            return cohort;
        }

        public void AddCohortToDb(Cohort cohortToAdd)
        {
            this.dbcontext.Cohorts.AddOrUpdate(cohortToAdd);
        }
    }
}
