using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Report
{
    public class ReportCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<ReportQuestion> Questions { get; set; }

    }
}