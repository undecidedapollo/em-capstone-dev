using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmployeeEvaluationSystem.MVC.Startup))]
namespace EmployeeEvaluationSystem.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
