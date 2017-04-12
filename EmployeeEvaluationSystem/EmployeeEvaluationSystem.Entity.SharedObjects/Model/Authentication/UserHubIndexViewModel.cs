using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication
{
    public class UserHubIndexViewModel
    {
        public IEnumerable<PendingSurvey> PendingSurveys { get; set; }
        public IEnumerable<PendingSurvey> FinishedSurveys { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Tuple<Question, AnswerInstance>> Questions { get; set; }
    }
}
