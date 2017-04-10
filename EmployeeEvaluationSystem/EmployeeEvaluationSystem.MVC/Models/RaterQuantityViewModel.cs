using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models
{
    public class RaterQuantityViewModel
    {

        public int Id { get; set; }

        public string DisplayName { get; set; }

        public int Quantity { get; set; }

        public bool CanChange { get; set; }

    }
}