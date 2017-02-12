using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models.Authentication
{
    public class MultiRegisterResult
    {
        public bool Successful { get; set; }
        public RegisterViewModel FailedUser { get; set; }
    }
}