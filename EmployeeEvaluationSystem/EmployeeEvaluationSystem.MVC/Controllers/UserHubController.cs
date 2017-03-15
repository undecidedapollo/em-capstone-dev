using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class UserHubController : Controller
    {
        // GET: UserHub
        public ActionResult Index()
        {
            return View();
        }
    }
}