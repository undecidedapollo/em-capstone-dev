using EmployeeEvaluationSystem.SharedObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Survey
{
    public class CreateAnswerInstanceModel
    {
        public QuestionTypeEnum QuestionType { get; set; }

        public int RatingResponse { get; set; }
    }
}
