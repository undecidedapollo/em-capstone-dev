using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Infrastructure
{
    public class DependencySynchonousInstance<T> : IDependencyReference<T>
    {
        private Func<T> invoker;

        private object theLock;

        public DependencySynchonousInstance(Func<T> invoker)
        {
            if (invoker == null)
            {
                throw new Exception();
            }

            this.invoker = invoker;
            this.theLock = new object();
        }


        public T GetDependency()
        {
            lock (theLock)
            {
                return this.invoker();
            }
        }
    }
}
