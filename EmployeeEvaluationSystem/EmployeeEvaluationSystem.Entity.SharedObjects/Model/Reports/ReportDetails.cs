using EmployeeEvaluationSystem.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports
{
    public class ReportDetails
    {
        public List<ReportGenerationViewModel> EmpAvgRatings { get; set; }
    }
}
