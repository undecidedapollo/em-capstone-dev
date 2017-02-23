using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Infrastructure
{
    public class Dependency
    {

        private static object dependencyLock = new object();

        private static Dependency dependency;

        private Dictionary<Type, IDependencyReference> dependencies;

        private Dependency()
        {
            dependencies = new Dictionary<Type, IDependencyReference>();
        }

        public static Dependency GetInstance()
        {
            lock (dependencyLock)
            {

                if(dependency == null)
                {
                    dependency = new Dependency();
                }

                return dependency;
            }
        }

        private void AddDependency(Type T, IDependencyReference reference)
        {
            lock (dependencyLock)
            {
                this.dependencies.Add(T, reference);
            }
        }

        private IDependencyReference<T> GetDependency<T>()
        {
            IDependencyReference reference = null;
            bool result;
            lock (dependencyLock)
            {
                result = this.dependencies.TryGetValue(typeof(T), out reference);
            }

            if (result == false)
            {
                throw new Exception("Unable to find a resource of the specified type.");
            }
            else
            {
                var newRef = reference as IDependencyReference<T>;

                if (newRef == null)
                {
                    throw new Exception("Unable to convert reference to appropriate type. This should not happen.");
                }

                return newRef;
            }
        }


        public static void RegisterInstance<T>(Func<T> invoker)
        {
            if(invoker == null)
            {
                throw new ArgumentNullException("The invoking function cannot be null.");
            }

            var newInstance = new DependencyInstance<T>(invoker);

            GetInstance().AddDependency(typeof(T), newInstance);
            
        }

        public static void RegisterSynchronousInstance<T>(Func<T> invoker)
        {
            if (invoker == null)
            {
                throw new ArgumentNullException("The invoking function cannot be null.");
            }

            var newInstance = new DependencySynchonousInstance<T>(invoker);

            GetInstance().AddDependency(typeof(T), newInstance);

        }

        public static void RegisterSingleton<T>(Func<T> invoker)
        {
            if (invoker == null)
            {
                throw new ArgumentNullException("The invoking function cannot be null.");
            }

            var newInstance = new DependencySingleton<T>(invoker);

            GetInstance().AddDependency(typeof(T), newInstance);
        }

        public static T FindMy<T>()
        {
            var invoker = GetInstance().GetDependency<T>();

            return invoker.GetDependency();
        }

        public static void ClearDependencies()
        {
            lock (dependencyLock)
            {
                GetInstance().dependencies.Clear();
            }
        }

        public static void ClearDependency<T>()
        {
            lock (dependencyLock)
            {
                GetInstance().dependencies.Remove(typeof(T));
            }
        }
    }
}
