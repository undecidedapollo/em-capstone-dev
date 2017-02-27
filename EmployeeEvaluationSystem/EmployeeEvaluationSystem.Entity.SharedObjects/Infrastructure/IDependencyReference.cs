using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Infrastructure
{
    public interface IDependencyReference<T> : IDependencyReference
    {
        T GetDependency();
    }

    public interface IDependencyReference
    {
    }
}
