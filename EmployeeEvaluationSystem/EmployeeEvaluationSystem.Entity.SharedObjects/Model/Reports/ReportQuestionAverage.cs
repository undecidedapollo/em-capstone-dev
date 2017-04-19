using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports
{
    public class ReportQuestionAverage
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public double RatingValue { get; set; }

    }
}
