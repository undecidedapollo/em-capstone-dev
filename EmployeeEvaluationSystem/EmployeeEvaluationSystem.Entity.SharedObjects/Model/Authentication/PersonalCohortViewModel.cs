using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication
{
    public class PersonalCohortViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime DateCreated { get; set; }

        public static PersonalCohortViewModel Convert(Cohort x)
        {
            if (x == null)
            {
                throw new Exception();
            }

            return new PersonalCohortViewModel()
            {
                ID = x.ID,
                Name = x.Name,
                Description = x.Description,
                DateCreated = x.DateCreated
            };
        }
    }
}

