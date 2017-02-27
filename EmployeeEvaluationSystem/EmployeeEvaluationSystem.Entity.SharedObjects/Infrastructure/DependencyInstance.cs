using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Infrastructure
{
    public class DependencyInstance<T> : IDependencyReference<T>
    {
        private Func<T> invoker;

        public DependencyInstance(Func<T> invoker)
        {
            if(invoker == null)
            {
                throw new Exception();
            }

            this.invoker = invoker;
        }


        public T GetDependency()
        {
            return this.invoker();
        }
    }
}
