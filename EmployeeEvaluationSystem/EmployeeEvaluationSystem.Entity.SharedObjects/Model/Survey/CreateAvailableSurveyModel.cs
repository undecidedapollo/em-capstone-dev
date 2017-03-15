using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Survey
{
    public class CreateAvailableSurveyModel
    {
        public int CohortId { get; set; }
        public int SurveyId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int SurveyTypeId { get; set; }

        public ICollection<CreateAvailableSurveyRolesModel> RolesSurveyFor { get; set; }

    }
}
