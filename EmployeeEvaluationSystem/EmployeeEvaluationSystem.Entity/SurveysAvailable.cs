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
    
    public partial class SurveysAvailable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SurveysAvailable()
        {
            this.PendingSurveys = new HashSet<PendingSurvey>();
            this.SurveysAvailableToes = new HashSet<SurveysAvailableTo>();
        }
    
        public int ID { get; set; }
        public int CohortID { get; set; }
        public int SurveyID { get; set; }
        public System.DateTime DateOpen { get; set; }
        public System.DateTime DateClosed { get; set; }
        public int SurveyTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateDeleted { get; set; }
        public bool IsCompleted { get; set; }
        public Nullable<System.DateTime> DateCompleted { get; set; }
    
        public virtual Cohort Cohort { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PendingSurvey> PendingSurveys { get; set; }
        public virtual Survey Survey { get; set; }
        public virtual SurveyType SurveyType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SurveysAvailableTo> SurveysAvailableToes { get; set; }
    }
}
