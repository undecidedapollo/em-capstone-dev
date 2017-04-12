using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class UserSurveyStatus
    {
        public Guid? Id { get; set; }

        public string NameOrEmail { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string Status { get; set; }

        public DateTime? DateStarted { get; set; }

        public DateTime? DateFinished { get; set; }

        public bool CanViewResults { get; set; }

        public bool CanResendEmail { get; set; }


    }
}