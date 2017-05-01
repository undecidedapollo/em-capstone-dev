using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Tests.Infrastructure
{
    public class PreExistingUnitOfWorkCreator : IUnitOfWorkCreator
    {
        public IUnitOfWork uow { get; set; }

        public IUnitOfWork Create()
        {
            return uow;
        }

        public void Dispose()
        {
         
        }
    }

    public static class UOWExtensions
    {
        public static PreExistingUnitOfWorkCreator GetCreator(this IUnitOfWork uow)
        {
            return new PreExistingUnitOfWorkCreator
            {
                uow = uow
            };
        }
    }
}
