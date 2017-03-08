﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class EmployeeDatabaseEntities : DbContext
    {
        public EmployeeDatabaseEntities()
            : base("name=EmployeeDatabaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<C__RefactorLog> C__RefactorLog { get; set; }
        public virtual DbSet<AnswerInstance> AnswerInstances { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Cohort> Cohorts { get; set; }
        public virtual DbSet<CohortPermission> CohortPermissions { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionType> QuestionTypes { get; set; }
        public virtual DbSet<SiteWidePermission> SiteWidePermissions { get; set; }
        public virtual DbSet<Survey> Surveys { get; set; }
        public virtual DbSet<SurveysAvailable> SurveysAvailables { get; set; }
        public virtual DbSet<SurveyType> SurveyTypes { get; set; }
        public virtual DbSet<UserSurveyRole> UserSurveyRoles { get; set; }
        public virtual DbSet<LocationAddress> LocationAddresses { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<CohortUser> CohortUsers { get; set; }
        public virtual DbSet<PendingSurvey> PendingSurveys { get; set; }
        public virtual DbSet<SurveyInstance> SurveyInstances { get; set; }
        public virtual DbSet<SurveysAvailableTo> SurveysAvailableToes { get; set; }
        public virtual DbSet<Status> Status { get; set; }
    
        public virtual int CancelAllOldSurveyLocks()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CancelAllOldSurveyLocks");
        }
    
        public virtual int CancelSurveyLock(Nullable<System.Guid> pendingSurveyId)
        {
            var pendingSurveyIdParameter = pendingSurveyId.HasValue ?
                new ObjectParameter("pendingSurveyId", pendingSurveyId) :
                new ObjectParameter("pendingSurveyId", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CancelSurveyLock", pendingSurveyIdParameter);
        }
    
        public virtual ObjectResult<LockAndGetSurvey_Result> LockAndGetSurvey(Nullable<System.Guid> pendingSurveyId, Nullable<System.Guid> statusGuid)
        {
            var pendingSurveyIdParameter = pendingSurveyId.HasValue ?
                new ObjectParameter("pendingSurveyId", pendingSurveyId) :
                new ObjectParameter("pendingSurveyId", typeof(System.Guid));
    
            var statusGuidParameter = statusGuid.HasValue ?
                new ObjectParameter("statusGuid", statusGuid) :
                new ObjectParameter("statusGuid", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LockAndGetSurvey_Result>("LockAndGetSurvey", pendingSurveyIdParameter, statusGuidParameter);
        }
    
        public virtual int UpdateLockedSurveyTime(Nullable<System.Guid> pendingSurveyId)
        {
            var pendingSurveyIdParameter = pendingSurveyId.HasValue ?
                new ObjectParameter("pendingSurveyId", pendingSurveyId) :
                new ObjectParameter("pendingSurveyId", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateLockedSurveyTime", pendingSurveyIdParameter);
        }
    }
}
