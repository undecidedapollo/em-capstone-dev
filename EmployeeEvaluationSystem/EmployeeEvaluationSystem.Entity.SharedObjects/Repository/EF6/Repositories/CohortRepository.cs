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
    public class CohortRepository : Repository, ICohortRepository
    {
        public CohortRepository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext) : base(unitOfWork, dbcontext)
        {

        }

        public virtual int GetNumberOfCohorts()
        {
            return this.dbcontext.Cohorts.Count();
        }

        public virtual IEnumerable<Cohort> GetAllCohorts(string userId)
        {
            return this.dbcontext.Cohorts;
        }

        public virtual Cohort GetCohort(string currentUserId, int? cohortIdToGet)
        {
            return this.dbcontext.Cohorts.FirstOrDefault(x => x.ID == cohortIdToGet && x.IsDeleted == false);
        }

        public virtual bool DeleteCohort(string currentUserId, int cohortIdToGet)
        {
            var cohort = this.GetCohort(currentUserId, cohortIdToGet);

            


            if (cohort == null)
            {
                throw new Exception();
            }

            var canDelete = cohort.SurveysAvailables.Any(x => x.IsDeleted == false && x.DateOpen <= DateTime.UtcNow);

            if (canDelete)
            {
                return false;
            }


            cohort.IsDeleted = true;
            cohort.DateDeleted = DateTime.UtcNow;
            return true;
        }

        public virtual Cohort EditCohort(string currentUserId, Cohort cohortToEdit)
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

        public virtual IEnumerable<AspNetUser> GetAllUsersThatAreNotPartOfACohort(string currentUserId)
        {
            var theCU = dbcontext.CohortUsers.Where(x => x.Cohort.IsDeleted == false).Select(x => x.AspNetUser).Distinct();


            return this.dbcontext.AspNetUsers.Except(theCU);

            //this.dbcontext.AspNetUsers.GroupJoin(dbcontext.CohortUsers, x => x.Id, x => x.UserID, (user, cu) => new { User = user, CU = cu });

            //return users;
        }

        public virtual void AddCohortToDb(string currentUserId, Cohort cohortToAdd)
        {
            this.dbcontext.Cohorts.Add(cohortToAdd);
        }
    }
}
