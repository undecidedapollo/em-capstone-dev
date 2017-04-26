using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core
{
    public interface IUnitOfWorkCreator : IDisposable
    {

        IUnitOfWork Create();
    }
}
