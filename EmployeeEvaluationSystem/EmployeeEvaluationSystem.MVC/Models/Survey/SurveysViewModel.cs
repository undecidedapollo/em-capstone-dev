using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class SurveysViewModel
    {
        public int Id { get; set; }

        public string SurveyName { get; set; }

        public string SurveyType { get; set; }

        public DateTime DateOpened { get; set; }

        public DateTime DateClosed { get; set; }

        public bool IsFinished { get; set; }


    }
}