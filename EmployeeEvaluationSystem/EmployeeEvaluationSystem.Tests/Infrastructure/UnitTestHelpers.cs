using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Tests.Infrastructure
{
    public static class UTH
    {
        public static T CheckIsTypeNotNullAndGetObj<T>(object x) where T: class
        {
            Assert.IsNotNull(x);

            Assert.IsInstanceOfType(x, typeof(T));
            T routeResult = x as T;

            Assert.IsNotNull(routeResult);
            return routeResult;
        }

        public static void IsTrue(params bool[] items)
        {
            Assert.IsTrue(items != null && !items.Any(x => x == false));
        }

        
    }
}
