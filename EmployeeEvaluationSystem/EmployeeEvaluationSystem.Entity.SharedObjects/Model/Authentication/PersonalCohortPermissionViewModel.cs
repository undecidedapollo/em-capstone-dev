using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication
{
    public class PersonalCohortPermissionViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Cohort { get; set; }
        public int User { get; set; }
        public int Survey { get; set; }
        public int Report { get; set; }

        public static PersonalCohortPermissionViewModel Convert(CohortPermission x)
        {
            if (x == null)
            {
                throw new Exception();
            }

            return new PersonalCohortPermissionViewModel()
            {
                ID = x.ID,
                Name = x.Name,
                Cohort = x.Cohort,
                User = x.User,
                Survey = x.Survey,
                Report = x.Report
            };
        }
    }
}
