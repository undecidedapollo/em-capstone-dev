using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Infrastructure
{
    public class DependencySingleton<T> : IDependencyReference<T>
    {
        private Func<T> invoker;
        private T theObject;
        private bool hasInvoked;
        private object theLock;

        public DependencySingleton(Func<T> invoker)
        {
            if (invoker == null)
            {
                throw new Exception();
            }

            this.invoker = invoker;
            this.theLock = new object();
            this.hasInvoked = false;
        }

        public T GetDependency()
        {
            lock (theLock)
            {
                if(hasInvoked == false)
                {
                    this.theObject = this.invoker();
                    hasInvoked = true;
                }
            }

            return this.theObject;
        }
    }
}
