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
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using Microsoft.AspNet.Identity.Owin;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    [Authorize(Roles = "Employee")]
    public class UserHubController : Controller
    {


        private IUnitOfWorkCreator creator;

        public IUnitOfWorkCreator Creator
        {
            get { return creator ?? HttpContext.GetOwinContext().Get<IUnitOfWorkCreator>(); }
            private set { creator = value; }
        }

        public UserHubController()
        {
        }

        public UserHubController(IUnitOfWorkCreator creator)
        {
            this.creator = creator;
        }

        // GET: UserHub
        public ActionResult Index(Guid? pendingSurveyID, int? categoryID)
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = this.Creator.Create())
            {
                var viewModel = new UserHubIndexViewModel()
                {
                    PendingSurveys = unitOfWork.Surveys.GetPendingSurveysForUser(userId),
                    FinishedSurveys = unitOfWork.Surveys.GetFinishedSurveysForUser(userId)
                };

                if (pendingSurveyID != null)
                {
                    ViewBag.PendingSurveyID = pendingSurveyID.Value;

                    var pendingSurvey = unitOfWork.Surveys.GetPendingSurvey(userId, pendingSurveyID.Value);

                    var surveyInstanceID = pendingSurvey.SurveyInstanceID;

                    var categories = pendingSurvey.SurveysAvailable.Survey.Categories;

                    viewModel.Categories = categories;

                    if (categoryID != null)
                    {
                        ViewBag.CategoryID = categoryID.Value;

                        viewModel.Questions = unitOfWork.Surveys.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(categoryID.Value, surveyInstanceID.Value);
                    }
                }

                return View(viewModel);
            }
        }
    }
}
