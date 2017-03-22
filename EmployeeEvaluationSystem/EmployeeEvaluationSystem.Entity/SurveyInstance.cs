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
    
    public partial class SurveyInstance
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SurveyInstance()
        {
            this.AnswerInstances = new HashSet<AnswerInstance>();
            this.PendingSurveys = new HashSet<PendingSurvey>();
        }
    
        public int ID { get; set; }
        public int SurveyID { get; set; }
        public string UserTakenBy { get; set; }
        public System.DateTime DateStarted { get; set; }
        public Nullable<System.DateTime> DateFinished { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnswerInstance> AnswerInstances { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Survey Survey { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PendingSurvey> PendingSurveys { get; set; }
    }
}
