using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models
{
    public class ConfirmEmailViewModel
    {
        public string userId { get; set; }
        public string emailCode { get; set; }
        public string password { get; set; }

        public string confirmpassword { get; set; }

    }
}