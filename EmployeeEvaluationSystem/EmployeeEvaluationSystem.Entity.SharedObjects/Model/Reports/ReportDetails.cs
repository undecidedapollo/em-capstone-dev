using System.Collections.Generic;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports
{
    public class ReportDetails
    {
        public List<ReportGenerationViewModel> EmpAvgRatings { get; set; }
        public List<UserRoleModel> UserRole { get; set; }
        public List<SurveyReportModel> SurveyReport { get; set; }
    }
}
