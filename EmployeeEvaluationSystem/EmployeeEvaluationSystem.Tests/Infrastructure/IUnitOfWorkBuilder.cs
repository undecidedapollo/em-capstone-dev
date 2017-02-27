using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Tests.Infrastructure
{
    public interface IUnitOfWorkBuilder<T>
    {
        IUnitOfWorkBuilder<T> InitializeOne<T1>(Expression<Func<T, IEnumerable<T1>>> x, IList<T1> data = null) where T1 : class;

        T GetContext();
    }
}
