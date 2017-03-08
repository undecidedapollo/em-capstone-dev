using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class AnswerViewModel
    {
        [Required]
        public int ResponseNum { get; set; }
    }
}