using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Report
{
    public class ReportDetailsViewModel
    {
        public List<ReportRole> ResponseItems { get; set; }

        public List<ReportCategory> Categories { get; set; }
    }
}