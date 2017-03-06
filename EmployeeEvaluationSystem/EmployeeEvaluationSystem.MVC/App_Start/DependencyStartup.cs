using EffectiveDependencyLibrary.Implementation.Standard;
using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.SharedObjects.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeEvaluationSystem.MVC
{
    public static class DependencyStartup
    {
        public static void Setup()
        {

            RegisterDependencies();
        }

        public static void RegisterResolver()
        {
            //TODO
        }

        public static void RegisterDependencies()
        {
            GlobalEffectiveDependency<DependencyType>.RegisterInstance(() => new EmployeeDatabaseEntities());
            GlobalEffectiveDependency<DependencyType>.RegisterInstance<IUnitOfWork>(() => new UnitOfWork(GlobalEffectiveDependency<DependencyType>.FindMy<EmployeeDatabaseEntities>()));
        }
    }
}