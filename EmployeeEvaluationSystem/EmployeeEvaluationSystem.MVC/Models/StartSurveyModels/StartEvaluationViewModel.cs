using EmployeeEvaluationSystem.Entity;
using System;
using System.Collections.Generic;

namespace EmployeeEvaluationSystem.MVC.Models
{
    public class StartEvaluationViewModel
    {
        public ICollection<EmployeeEvaluationSystem.Entity.Survey> Surveys { get; set; }
        public ICollection<SurveyType> SurveyTypes { get; set; }
        public IList<RaterQuantityViewModel> RoleQuantities { get; set; }
        public IList<(EmployeeEvaluationSystem.Entity.Survey, SurveyType, SurveyState)?> AssignedSurveys { get; set; }
        public IList<CSSurveyViewModel> NewSurveys { get; set; }
        public DateTime DateOpen { get; set; }
        public DateTime DateClosed { get; set; }
        public enum SurveyState { COMPLETE, IN_PROGRESS, AVAILABLE, NOT_AVAILABLE }
        public int CohortID { get; set; }
        public int SurveyTypeID { get; set; }
        public int SurveyID { get; set; }
    }
}