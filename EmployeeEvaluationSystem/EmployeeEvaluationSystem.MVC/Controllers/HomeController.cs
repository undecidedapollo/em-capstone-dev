using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Example of unit of work framework
            using (var unitOfWork = new UnitOfWork())
            {
                var numUsers = unitOfWork.Users.GetNumberOfUsers();
                ViewBag.NumUsers = numUsers;
            }

            return View();
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