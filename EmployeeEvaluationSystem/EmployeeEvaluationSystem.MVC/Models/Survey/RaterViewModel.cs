using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class RaterViewModel
    {
        public string Email { get; set; }

        public int RoleId { get; set; }

        public string Role { get; set; }

        public string Status { get; set; }

        public bool CanChange { get; set; }

        public Guid Id { get; set; }

        public bool CanResendEmail { get; set; }

        public string RaterFirstName { get; set; }
        public string RaterLastName { get; set; }


    }
}