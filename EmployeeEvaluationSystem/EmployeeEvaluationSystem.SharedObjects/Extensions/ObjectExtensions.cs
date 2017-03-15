using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.SharedObjects.Extensions
{
    public static class ObjectExtensions
    {
        public static T ThrowIfNull<T>(this T theObj) where T : class
        {
            if(theObj == null)
            {
                throw new Exception("The object is null.");
            }

            var returnVal = theObj as T;

            if (returnVal == null)
            {
                throw new Exception("The object is null.");
            }

            return returnVal;
        }

        public static T ThrowIfNull<T, U>(this T theObj, U e) where T : class where U : Exception
        {
            if (theObj == null)
            {
                throw e;
            }

            var returnVal = theObj as T;

            if (returnVal == null)
            {
                throw e;
            }

            return returnVal;
        }
    }
}
