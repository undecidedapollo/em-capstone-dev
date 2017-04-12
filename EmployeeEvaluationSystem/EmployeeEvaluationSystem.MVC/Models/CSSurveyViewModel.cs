using EmployeeEvaluationSystem.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static EmployeeEvaluationSystem.MVC.Models.StartEvaluationViewModel;

namespace EmployeeEvaluationSystem.MVC.Models
{
    public class CSSurveyViewModel
    {
        public EmployeeEvaluationSystem.Entity.Survey TheSurvey { get; set; }

        public SurveyType TheSurveyType { get; set; }

        public SurveyState TheState { get; set; }
    }
}