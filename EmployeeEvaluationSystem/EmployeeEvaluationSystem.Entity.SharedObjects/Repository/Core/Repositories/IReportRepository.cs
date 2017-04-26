using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories
{
    public interface IReportRepository : IRepository
    {
        List<ReportRole> GetDetailsForReport(string userId, int surveyAvailableId);

        List<ReportRole> GetDetailsForReport(int surveyAvailableId);
    }
}
