using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6
{
    public class UnitOfWorkCreator : IUnitOfWorkCreator
    {
        public IUnitOfWork Create()
        {
            return new UnitOfWork(new EmployeeDatabaseEntities());
        }

        public void Dispose()
        {
            
        }
    }
}
