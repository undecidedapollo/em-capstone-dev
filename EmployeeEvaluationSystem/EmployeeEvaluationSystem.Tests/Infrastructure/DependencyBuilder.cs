using EffectiveDependencyLibrary.Implementation.Standard;
using EffectiveDependencyLibrary.Interfaces;
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
    public static class DependencyBuilder
    {
        public static Tuple<EffectiveDependency, object> BuildDependency()
        {
            var x = new object();
            GlobalEffectiveDependency.GetGlobalInstance(x).RegisterSingleton(() => EFUnitOfWorkBuilder<EmployeeDatabaseEntities>.Create());
            GlobalEffectiveDependency.GetGlobalInstance(x).RegisterInstance(() => GlobalEffectiveDependency.GetGlobalInstance(x).FindMy<IUnitOfWorkBuilder<EmployeeDatabaseEntities>>().GetContext());
            GlobalEffectiveDependency.GetGlobalInstance(x).RegisterInstance<IUnitOfWork>(() => new UnitOfWork(GlobalEffectiveDependency.GetGlobalInstance(x).FindMy<EmployeeDatabaseEntities>()));

            return Tuple.Create(GlobalEffectiveDependency.GetGlobalInstance(x), x);
        }
    }
}
