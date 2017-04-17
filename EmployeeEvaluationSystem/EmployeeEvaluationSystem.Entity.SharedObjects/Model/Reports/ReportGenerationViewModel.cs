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
        //public string Stage { get; set; }


        //public virtual CohortUser CohortId { get; set; }
        //public virtual CohortUser CohortUserId { get; set; }
        //public virtual AspNetUser EmployeeId { get; set; }
        //public virtual AnswerInstance QuestionId { get; set; }
        //public virtual AnswerInstance RatingValue { get; set; }
        //public virtual Category CategoryName { get; set; }
        //public virtual AspNetUser Username { get; set; }
       
    }
    
}