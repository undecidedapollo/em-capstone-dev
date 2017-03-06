using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication
{
    public class PersonalPermissionViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }

        public static PersonalPermissionViewModel Convert(Permission x)
        {
            if (x == null)
            {
                throw new Exception();
            }

            return new PersonalPermissionViewModel()
            {
                ID = x.ID,
                Name = x.Name,
                Add = x.Add,
                Edit = x.Edit,
                Delete = x.Delete,
                View = x.View
            };
        }
    }
}
