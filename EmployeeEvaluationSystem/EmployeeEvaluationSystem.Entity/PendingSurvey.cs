//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EmployeeEvaluationSystem.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class PendingSurvey
    {
        public System.Guid Id { get; set; }
        public int SurveyAvailToMeID { get; set; }
        public int UserSurveyRoleID { get; set; }
        public System.DateTime DateSent { get; set; }
        public Nullable<int> SurveyInstanceID { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DateDeleted { get; set; }
        public string UserSurveyForId { get; set; }
        public string UserTakenById { get; set; }
        public int StatusId { get; set; }
        public Nullable<System.Guid> StatusGuid { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string RaterFirstName { get; set; }
        public string RaterLastName { get; set; }
    
        public virtual AspNetUser UserSurveyFor { get; set; }
        public virtual AspNetUser UserTakenBy { get; set; }
        public virtual SurveyInstance SurveyInstance { get; set; }
        public virtual UserSurveyRole UserSurveyRole { get; set; }
        public virtual Status Status { get; set; }
        public virtual SurveysAvailable SurveysAvailable { get; set; }
    }
}
