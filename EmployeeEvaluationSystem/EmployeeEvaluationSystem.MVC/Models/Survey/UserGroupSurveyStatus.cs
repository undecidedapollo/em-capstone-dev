using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Survey
{
    public class UserGroupSurveyStatus
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public IList<UserSurveyStatus> UsersForSurvey { get; set; }
    }
}