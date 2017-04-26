using System.Web.Mvc;
using EmployeeEvaluationSystem.MVC.Infrastructure;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class HomeController : ADependencyMVCControllerHelper
    {

        public ActionResult Index()
        {
            return RedirectToAction("RedirectToLocal", "Account");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}