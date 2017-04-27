using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class SurveyDetailsViewModel
    {
        public SurveysViewModel TheSurvey { get; set; }

        public IList<UserGroupSurveyStatus> UserGroups { get; set; }

        public bool CanShowAllReport { get; set; }


    }
}