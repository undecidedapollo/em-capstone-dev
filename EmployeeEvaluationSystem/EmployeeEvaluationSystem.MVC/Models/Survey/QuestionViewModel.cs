using EmployeeEvaluationSystem.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class QuestionViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }

        public string DisplayText { get; set; }

        public bool IsRequired { get; set; }

        public QuestionTypeViewModel QuestionType { get; set; }

        public static QuestionViewModel Convert(Question x)
        {
            if(x == null)
            {
                return null;
            }

            return new QuestionViewModel
            {
                Id = x.ID,
                Name = x.Name,
                DisplayText = x.DisplayText,
                IsRequired = x.IsRequired,
                QuestionType = QuestionTypeViewModel.Convert(x.QuestionType)
            };
        }

    }
}