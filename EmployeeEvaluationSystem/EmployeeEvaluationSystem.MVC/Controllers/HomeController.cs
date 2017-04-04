using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EffectiveDependencyLibrary.Implementation.Standard;
using EmployeeEvaluationSystem.SharedObjects.Dependency;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EffectiveDependencyLibrary.Interfaces;
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