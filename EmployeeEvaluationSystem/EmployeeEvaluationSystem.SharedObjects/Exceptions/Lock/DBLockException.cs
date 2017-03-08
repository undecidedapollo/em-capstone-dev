using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.SharedObjects.Exceptions.Lock
{
    public class DBLockException : EESException
    {
        public DBLockException(): base()
        {

        }

        public DBLockException(string message) : base(message)
        {

        }
    }
}
