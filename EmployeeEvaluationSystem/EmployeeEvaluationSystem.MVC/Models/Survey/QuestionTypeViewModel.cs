using EmployeeEvaluationSystem.Entity;
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

        public static QuestionTypeViewModel Convert(QuestionType x)
        {
            if(x == null)
            {
                return null;
            }

            return new QuestionTypeViewModel
            {
                Id = x.ID,
                IsRating = x.IsRating,
                RatingMax = x.RatingMax,
                RatingMin = x.RatingMin
            };
        }

    }
}