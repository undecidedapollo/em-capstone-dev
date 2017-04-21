using EmployeeEvaluationSystem.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports
{
    public class ReportGenerationViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UserRoleModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class SurveyReportModel
    {
        public int ID { get; set; }
        public int QuestionID { get; set; }
        public int RatingValue { get; set; }

    }
    
}