using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class RatersPageViewModel
    {
        public Guid PendingSurveyId { get; set; }

        public List<RaterViewModel> Raters { get; set; } = new List<RaterViewModel>();
    }
}