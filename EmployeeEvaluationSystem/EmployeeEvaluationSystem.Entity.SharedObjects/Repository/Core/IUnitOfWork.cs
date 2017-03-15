using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core
{
    public interface IUnitOfWork: IDisposable
    {





        IUserRepository Users { get; }
        ICohortRepository Cohorts { get; }
        ICohortUserRepository CohortUsers { get; }
        ICohortPermissionRepository CohortPermissions { get; }
        IPermissionRepository Permissions { get; }

        ISurveyRepository Surveys { get; }

        int Complete();
    }
}
