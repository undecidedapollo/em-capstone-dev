using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.SharedObjects.Exceptions.Validitity
{
    public class InvalidModelException : EESException
    {
        public InvalidModelException(): base()
        {

        }

        public InvalidModelException(string message) : base(message)
        {

        }  
    }
}
