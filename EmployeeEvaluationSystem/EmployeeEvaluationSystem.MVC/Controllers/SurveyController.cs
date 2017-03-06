using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class SurveyController : Controller
    {

        public ActionResult StartSurvey(int pendingSurveyId)
        {




            return View();
        }

        public ActionResult ContinueSurvey()
        {
            return View();
        }

        public ActionResult SurveyPage()
        {
            return View();
        }

        public ActionResult EndSurvey()
        {
            return View();
        }
    }
}