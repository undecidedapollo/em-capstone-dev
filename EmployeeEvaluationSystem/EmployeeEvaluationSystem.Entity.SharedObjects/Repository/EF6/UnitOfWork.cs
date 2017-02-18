using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6
{
    public class UnitOfWork : IUnitOfWork
    {
        private EmployeeDatabaseEntities dbContext;

        public UnitOfWork(EmployeeDatabaseEntities dbContext)
        {
            this.dbContext = dbContext;
            Initialize();
        }

        public UnitOfWork()
        {
            this.dbContext = new EmployeeDatabaseEntities();
            Initialize();
        }

        private void Initialize()
        {
            this.Users = new UserRepository(this, this.dbContext);
            this.Cohorts = new CohortRepository(this, this.dbContext);
            this.CohortUsers = new CohortUserRepository(this, this.dbContext);
        }


        public IUserRepository Users { get; private set; }
        public ICohortRepository Cohorts { get; private set; }
        public ICohortUserRepository CohortUsers { get; private set; }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }

        public int Complete()
        {

            return this.dbContext.SaveChanges();
        }
    }
}
