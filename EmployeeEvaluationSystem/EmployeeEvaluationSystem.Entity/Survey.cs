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
    
    public partial class Survey
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Survey()
        {
            this.Categories = new HashSet<Category>();
            this.PendingSurveys = new HashSet<PendingSurvey>();
            this.SurveyInstances = new HashSet<SurveyInstance>();
            this.SurveysAvailables = new HashSet<SurveysAvailable>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateDeleted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PendingSurvey> PendingSurveys { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SurveyInstance> SurveyInstances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SurveysAvailable> SurveysAvailables { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
