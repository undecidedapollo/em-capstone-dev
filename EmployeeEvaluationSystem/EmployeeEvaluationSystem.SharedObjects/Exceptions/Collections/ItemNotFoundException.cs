using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.SharedObjects.Exceptions.Collections
{
    public class ItemNotFoundException : EESException
    {
        public ItemNotFoundException(): base()
        {

        }

        public ItemNotFoundException(string message) : base(message)
        {

        }
    }
}
