using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using EmployeeEvaluationSystem.Entity;

namespace EmployeeEvaluationSystem.MVC.Models
{
    public class CreateCohortViewModel
    {
        public PersonalCohortViewModel PersonalCohortViewModel { get; set; }
        public List<PersonalAspNetUserViewModel> Users { get; set; }

        public CreateCohortViewModel(List<PersonalAspNetUserViewModel> users)
        {
            Users = users;
            PersonalCohortViewModel = new PersonalCohortViewModel();
        }
    }
}