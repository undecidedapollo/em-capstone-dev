using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    public class Repository : IRepository
    {
        protected UnitOfWork unitOfWork { get; private set; }
        protected EmployeeDatabaseEntities dbcontext { get; private set; }

        public Repository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext)
        {
            this.unitOfWork = unitOfWork;
            this.dbcontext = dbcontext;
        }
    }
}
