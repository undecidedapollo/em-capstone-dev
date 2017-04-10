﻿using EmployeeEvaluationSystem.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models
{
    public class StartEvaluationViewModel
    {
        public ICollection<EmployeeEvaluationSystem.Entity.Survey> Surveys { get; set; }
        public ICollection<SurveyType> SurveyTypes { get; set; }
        public IList<RaterQuantityViewModel> RoleQuantities { get; set; }
        public DateTime DateOpen { get; set; }
        public DateTime DateClosed { get; set; }
    }
}