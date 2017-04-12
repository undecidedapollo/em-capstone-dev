using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.MVC.Models.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models
{
    public class CohortDetailsViewmodel
    {
        public Cohort TheCohort { get; set; }

        public IList<SurveysViewModel> Surveys { get; set; }
    }
}