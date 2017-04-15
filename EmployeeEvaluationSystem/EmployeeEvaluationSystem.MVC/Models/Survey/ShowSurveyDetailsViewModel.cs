using EmployeeEvaluationSystem.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class ShowSurveyDetailsViewModel
    {
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserForEmail { get; set; }
        public string UserForName { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateCompleted { get; set; }
        public string SurveyName { get; set; }
        public string SurveyType { get; set; }
        public string UserRole { get; set; }



        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Tuple<Question, AnswerInstance>> Questions { get; set; }
    }
}