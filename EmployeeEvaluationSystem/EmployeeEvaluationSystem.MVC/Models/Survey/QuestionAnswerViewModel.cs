using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class QuestionAnswerViewModel
    {
        public QuestionViewModel Question { get; set; }

        public AnswerViewModel Answer { get; set; }
    }
}