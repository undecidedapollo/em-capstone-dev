using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string DisplayText { get; set; }

        public bool IsRequired { get; set; }

        public QuestionTypeViewModel QuestionType { get; set; }

    }
}