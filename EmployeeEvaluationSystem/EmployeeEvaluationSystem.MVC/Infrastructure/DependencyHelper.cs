using EffectiveDependencyLibrary.Implementation.Standard;
using EffectiveDependencyLibrary.Interfaces;
using EmployeeEvaluationSystem.SharedObjects.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeEvaluationSystem.MVC.Infrastructure
{
    public static class DependencyHelper
    {
        public static IDependencyManager GetDI(IDependencyHelper x)
        {
            if(x == null)
            {
                throw new ArgumentNullException();
            }

            if (x.DependencyObject != null)
            {
                return GlobalEffectiveDependency.GetGlobalInstance(x.DependencyObject);
            }
            else
            {
                return GlobalEffectiveDependency<DependencyType>.GetInstance();
            }
        }
    }

    public abstract class ADependencyMVCControllerHelper : Controller, IDependencyHelper
    {
        public object DependencyObject { get; set; }

        public IDependencyManager GetDI()
        {
            return DependencyHelper.GetDI(this);
        }
    }

    public abstract class ADependencyHelper : IDependencyHelper
    {
        public object DependencyObject { get; set; }

        public IDependencyManager GetDI()
        {
            return DependencyHelper.GetDI(this);
        }
    }

    public interface IDependencyHelper
    {
        object DependencyObject { get; set; }

        IDependencyManager GetDI();
    }
}