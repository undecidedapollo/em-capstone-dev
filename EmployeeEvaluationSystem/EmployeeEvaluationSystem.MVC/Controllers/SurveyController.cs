using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.MVC.Models.Survey;
using EmployeeEvaluationSystem.SharedObjects.Extensions;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class SurveyController : Controller
    {

        public ActionResult StartSurvey(Guid pendingSurveyId, string email = null)
        {
            var userId = User?.Identity?.GetUserId();


            if(userId == null && email == null)
            {
                throw new Exception();
            }

            bool guestMode = userId == null;

            using (var unitOfWork = new UnitOfWork())
            {
                bool canTake = guestMode ? unitOfWork.Surveys.CanGuestUserTakeSurvey(email, pendingSurveyId): unitOfWork.Surveys.CanExistingUserTakeSurvey(userId, pendingSurveyId);

                if (!canTake)
                {
                    throw new Exception();
                }
            }

            return View();
        }

        public ActionResult ContinueSurvey()
        {
            return View();
        }

        public ActionResult SurveyPage(Guid pendingSurveyId, string email = null)
        {
            var userId = User?.Identity?.GetUserId();


            if (userId == null && email == null)
            {
                throw new Exception();
            }

            bool guestMode = userId == null;

            using (var unitOfWork = new UnitOfWork())
            {
                bool canTake = guestMode ? unitOfWork.Surveys.CanGuestUserTakeSurvey(email, pendingSurveyId) : unitOfWork.Surveys.CanExistingUserTakeSurvey(userId, pendingSurveyId);

                if (!canTake)
                {
                    throw new Exception();
                }

                var pendingSurvey = unitOfWork.Surveys.GetPendingSurveySYSTEM(pendingSurveyId).ThrowIfNull();



            }

            return View();
        }

        public ActionResult SurveyPage(SurveyPageViewModel model)
        {
            return View();
        }

        public ActionResult EndSurvey()
        {
            return View();
        }
    }
}