using System;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using Microsoft.AspNet.Identity;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    [Authorize(Roles = "Employee")]
    public class UserHubController : Controller
    {

        // GET: UserHub
        public ActionResult Index(Guid? pendingSurveyID, int? categoryID)
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
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
