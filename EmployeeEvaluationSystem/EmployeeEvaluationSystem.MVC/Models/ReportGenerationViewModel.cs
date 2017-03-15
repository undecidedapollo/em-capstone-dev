using EmployeeEvaluationSystem.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeEvaluationSystem.MVC.Models
{
    public class ReportGenerationViewModel
    {
        public decimal OverallAverage { get; set; }
        public decimal SuperviseeAverage { get; set; }
        public decimal CoworkerAverage { get; set; }
        public int SupervisorEvaluationResult { get; set; }
        public int SelfEvaluationResult { get; set; }
        public string Stage { get; set; }


        public CohortUser CohortId { get; set; }
        public CohortUser CohortUserId { get; set; }
        public AspNetUser EmployeeId { get; set; }
        public AnswerInstance QuestionId { get; set; }
        public AnswerInstance RatingValue { get; set; }
        public Category CategoryName { get; set; }
        public AspNetUser Username { get; set; }
       
    }
}