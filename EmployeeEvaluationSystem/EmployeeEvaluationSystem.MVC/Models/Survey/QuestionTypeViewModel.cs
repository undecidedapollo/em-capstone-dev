using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class QuestionTypeViewModel
    {
        public int Id { get; set; }
        public bool IsRating { get; set; }
        public int RatingMax { get; set; }
        public int RatingMin { get; set; }

    }
}