using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Helpers.Locks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Survey;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.MVC.Models.Survey;
using EmployeeEvaluationSystem.SharedObjects.Exceptions.Lock;
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


                var lockPendingSurvey = unitOfWork.Surveys.LockAndGetSurvey(pendingSurveyId);

                if (lockPendingSurvey == null)
                {
                    throw new DBLockException();
                }

                if (lockPendingSurvey.StatusGuid == null)
                {
                    throw new Exception();
                }

                var dbPendingSurvey = unitOfWork.Surveys.GetPendingSurveySYSTEM(pendingSurveyId).ThrowIfNull();

                var instanceSurvey = dbPendingSurvey.SurveyInstance;

                SurveyInstance theInstance = null;

                if (instanceSurvey == null)
                {
                    if (guestMode)
                    {
                        theInstance = unitOfWork.Surveys.CreateSurveyInstanceForGuestUser(email, pendingSurveyId);
                    }
                    else
                    {
                        theInstance = unitOfWork.Surveys.CreateSurveyInstanceForExistingUser(userId, pendingSurveyId);
                    }

                    unitOfWork.Complete();

                    theInstance = unitOfWork.Surveys.GetSurveyInstanceByIdSYSTEM(theInstance.ID); //Need to get categories.
                }
                else if (instanceSurvey.DateFinished == null)
                {
                    theInstance = instanceSurvey;
                }
                else
                {
                    throw new Exception(); //The survey is already finished.
                }



                var firstCategory = unitOfWork.Surveys.GetFirstCategory(theInstance.SurveyID);




                var viewModel = new SurveyPageViewModel
                {
                    SurveyInstanceId = theInstance.ID,
                    PendingSurveyId = pendingSurveyId,
                    StatusGuid = lockPendingSurvey.StatusGuid ?? Guid.NewGuid(),
                    Category = CategoryViewModel.Convert(firstCategory),
                    Questions = firstCategory.Questions.Select(x => new QuestionAnswerViewModel { Question = QuestionViewModel.Convert(x) }).ToList()
                };

                return View("SurveyPage", viewModel);
            }

        }

        [HttpPost]
        public ActionResult SurveyPage(SurveyPageViewModel model)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var theSurvey = unitOfWork.Surveys.LockAndGetSurvey(model.PendingSurveyId, model.StatusGuid);

                var surveyInstance = unitOfWork.Surveys.GetSurveyInstanceByIdSYSTEM(model.SurveyInstanceId);

                foreach(var qa in model.Questions)
                {
                    var newAnswerInstanceModel = new CreateAnswerInstanceModel
                    {
                        RatingResponse = qa.Answer.ResponseNum
                    };

                    unitOfWork.Surveys.AddAnswerInstanceToSurveyInstance(model.SurveyInstanceId, qa.Question.Id, newAnswerInstanceModel);
                }

                unitOfWork.Complete();

                var nextCategory = unitOfWork.Surveys.GetNextCategory(surveyInstance.SurveyID);




                var viewModel = new SurveyPageViewModel
                {
                    SurveyInstanceId = model.SurveyInstanceId,
                    PendingSurveyId = model.PendingSurveyId,
                    StatusGuid = theSurvey.StatusGuid ?? Guid.NewGuid(),
                    Category = CategoryViewModel.Convert(nextCategory),
                    Questions = nextCategory.Questions.Select(x => new QuestionAnswerViewModel { Question = QuestionViewModel.Convert(x) }).ToList()
                };
            }

            return View();
        }

        public ActionResult EndSurvey()
        {
            return View();
        }
    }
}