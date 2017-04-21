using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Report
{
    public class ReportDetailsViewModel
    {
        public string Header { get; set; }

        public string Header2 { get; set; }
        public List<ReportRole> ResponseItems { get; set; }

        public List<ReportCategory> Categories { get; set; }
    }
}