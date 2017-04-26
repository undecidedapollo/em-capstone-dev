using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Tests.Infrastructure
{
    public class TestUnitOfWorkCreator : IUnitOfWorkCreator
    {
        public EmployeeDatabaseEntities entity {get; set;}

        public IUnitOfWork Create()
        {
            return new UnitOfWork(entity);
        }

        public void Dispose()
        {
            if(this.entity != null)
            {
                this.entity.Dispose();
            }
        }
    }

    public static class BuilderExtensions
    {
        public static TestUnitOfWorkCreator GetCreator(this IUnitOfWorkBuilder<EmployeeDatabaseEntities> builder) 
        {
            return new TestUnitOfWorkCreator
            {
                entity = builder.GetContext()
            };
        }
    }
}
