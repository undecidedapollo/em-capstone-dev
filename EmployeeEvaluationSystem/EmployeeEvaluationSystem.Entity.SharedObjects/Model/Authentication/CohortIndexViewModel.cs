using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication
{
    public class CohortIndexViewModel
    {
        public IEnumerable<PersonalCohortViewModel> Cohorts { get; set; }
        public IEnumerable<PersonalAspNetUserViewModel> Users { get; set; }
    }
}
