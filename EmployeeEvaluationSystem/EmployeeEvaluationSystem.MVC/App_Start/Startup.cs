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
            ConfigureAuth(app);
            DependencyStartup.Setup();

            GlobalConfiguration.Configuration.UseSqlServerStorage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
