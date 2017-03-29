using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using Microsoft.AspNet.Identity;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class UserHubController : Controller
    {

        // GET: UserHub
        public ActionResult Index()
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var viewModel = new UserHubIndexViewModel()
                {
                    PendingSurveys = unitOfWork.Surveys.GetPendingSurveysForUser(userId),
                    FinishedSurveys = unitOfWork.Surveys.GetFinishedSurveysForUser(userId)
                };

                return View(viewModel);
            }
        }
    }
}
