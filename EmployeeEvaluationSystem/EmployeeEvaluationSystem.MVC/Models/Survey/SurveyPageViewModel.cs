using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class SurveyPageViewModel
    {
        [Required]
        public int SurveyInstanceId { get; set; }

        [Required]
        public Guid PendingSurveyId { get; set; }

        [Required]
        public Guid StatusGuid { get; set; }
        [Required]
        public CategoryViewModel Category { get; set; }

        [Required]
        public List<QuestionAnswerViewModel> Questions { get; set; } = new List<QuestionAnswerViewModel>();
    }
}