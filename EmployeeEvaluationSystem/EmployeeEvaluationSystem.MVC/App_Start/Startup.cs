using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.MVC.Infrastructure.Hangfire;
using Hangfire;
using Microsoft.Owin;
using Owin;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(EmployeeEvaluationSystem.MVC.Startup))]
namespace EmployeeEvaluationSystem.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<IUnitOfWorkCreator>(() => { return new UnitOfWorkCreator(); });

            ConfigureAuth(app);

            GlobalConfiguration.Configuration.UseSqlServerStorage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate(() => SurveyHelper.CancelOldSurveyLocks(), "*/2 * * * *");
            RecurringJob.AddOrUpdate(() => SurveyHelper.SetExpiredSurveysToCompleted(), Cron.MinuteInterval(10));
        }
    }
}
