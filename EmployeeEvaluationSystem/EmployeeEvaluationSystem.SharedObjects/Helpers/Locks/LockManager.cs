using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.SharedObjects.Helpers.Locks
{
    public class LockManager<T> : ILockManager<T>
    {
        private Func<T> before;
        private T beforeValue;
        private Action after;
        

        public LockManager(Func<T> before, Action after){
            this.before = before;
            this.after = after;
        }


        public void After()
        {
            this.after();
        }

        public void Before()
        {
            this.beforeValue = this.before();
        }

        public T BeforeValue() => this.beforeValue;
    }
}
