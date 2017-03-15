using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.SharedObjects.Helpers.Locks
{
    public interface ILockManager<T>
    {
        void Before();

        T BeforeValue();

        void After();

    }
}
