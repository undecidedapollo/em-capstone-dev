using System.Web.Mvc;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class ErrorPageController : Controller
    {
        public ActionResult Oops(int id)
        {
            Response.StatusCode = id;

            return View();
        }
    }
}