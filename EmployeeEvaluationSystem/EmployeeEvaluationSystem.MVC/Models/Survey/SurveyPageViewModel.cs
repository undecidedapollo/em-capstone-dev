using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class SurveyPageViewModel
    {
        public int SurveyInstanceId { get; set; }
        public CategoryViewModel Category { get; set; }

        public List<QuestionAnswerViewModel> Questions { get; set; } = new List<QuestionAnswerViewModel>();
    }
}