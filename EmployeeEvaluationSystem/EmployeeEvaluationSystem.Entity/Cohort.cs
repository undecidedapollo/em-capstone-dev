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
    
    public partial class Cohort
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cohort()
        {
            this.SurveysAvailables = new HashSet<SurveysAvailable>();
            this.CohortUsers = new HashSet<CohortUser>();

            this.DateCreated = DateTime.Now;
            this.IsDeleted = false;
            this.DateDeleted = null;
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DateDeleted { get; set; }
        public System.DateTime DateCreated { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SurveysAvailable> SurveysAvailables { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CohortUser> CohortUsers { get; set; }
    }
}
