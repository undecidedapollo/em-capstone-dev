using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Helpers.Locks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Survey;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.MVC.Models.Survey;
using EmployeeEvaluationSystem.SharedObjects.Exceptions.Lock;
using EmployeeEvaluationSystem.SharedObjects.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class SurveyController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private HttpRequestBase passedInRequest;

        public SurveyController()
        {
        }

        public SurveyController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, HttpRequestBase request = null)
        {
            this.passedInRequest = request;
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        //[Route("Survey/StartSurvey/{pendingSurveyId}/{email?}")]
        public ActionResult StartSurvey(Guid pendingSurveyId, string email = null, string userId = null)
        {
            var identityUserId = User?.Identity?.GetUserId();

            if (identityUserId == null)
            {
                if (userId == null && email != null)
                {

                }
                if (userId != null && email == null)
                {
                    using (var unitOfWork = new UnitOfWork())
                    {
                        var auth = unitOfWork.Surveys.CanExistingUserTakeSurvey(userId, pendingSurveyId);

                        if (!auth) throw new UnauthorizedAccessException();
                    }

                    var user = this.UserManager.FindById(userId) ?? throw new UnauthorizedAccessException();
                    this.SignInManager.SignIn(user, true, false);
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                userId = identityUserId;
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

        //[Route("Survey/SurveyPage/{pendingSurveyId}/{email?}")]
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
                    return RedirectToAction("SurveyLocked");
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
                var alreadyAnsweredQuestions = unitOfWork.Surveys.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(firstCategory.ID, theInstance.ID);



                var viewModel = new SurveyPageViewModel
                {
                    SurveyInstanceId = theInstance.ID,
                    PendingSurveyId = pendingSurveyId,
                    StatusGuid = lockPendingSurvey.StatusGuid ?? Guid.NewGuid(),
                    Category = CategoryViewModel.Convert(firstCategory),
                    Questions = alreadyAnsweredQuestions.Select(x => new QuestionAnswerViewModel { Question = QuestionViewModel.Convert(x.Item1), Answer = new AnswerViewModel { ResponseNum = x?.Item2?.ResponseNum ?? null } }).ToList()
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

                if (theSurvey == null) return RedirectToAction("SurveyLocked"); ;

                var surveyInstance = unitOfWork.Surveys.GetSurveyInstanceByIdSYSTEM(model.SurveyInstanceId);

                var hasShownRequired = false;

                if (model.BackOnePage == false)
                {
                    try
                    {
                        foreach (var qa in model.Questions)
                        {
                            if (qa?.Answer?.ResponseNum == null)
                            {
                                var isRequired = unitOfWork.Surveys.IsQuestionRequired(qa?.Question?.Id ?? throw new Exception());

                                if (isRequired)
                                {
                                    if (!hasShownRequired)
                                    {
                                        hasShownRequired = true;
                                    }

                                }

                                continue;
                            }

                            var newAnswerInstanceModel = new CreateAnswerInstanceModel
                            {
                                RatingResponse = qa.Answer.ResponseNum ?? -1
                            };

                            unitOfWork.Surveys.AddAnswerInstanceToSurveyInstance(model.SurveyInstanceId, qa.Question.Id, newAnswerInstanceModel);
                        }

                        unitOfWork.Complete();

                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "There was an error saving the answer's to your survey. Please correct any mistakes and try again!");
                        return View(model);
                    }
                }





                Category nextCategory = null;

                if (model?.Category?.Id == null)
                {
                    nextCategory = unitOfWork.Surveys.GetLastCategory(surveyInstance.SurveyID);
                }
                else if (model.BackOnePage)
                {
                    nextCategory = unitOfWork.Surveys.GetPreviousCategory(model.Category.Id);
                }
                else
                {
                    nextCategory = unitOfWork.Surveys.GetNextCategory(model.Category.Id);
                }




                if (nextCategory == null)
                {
                    return RedirectToAction("EndSurvey", new { SurveyInstanceId = model.SurveyInstanceId, penSurveyId = model.PendingSurveyId, statGuid = model.StatusGuid });
                }


                ModelState.Clear();


                if (hasShownRequired)
                {
                    ModelState.AddModelError("", "All required questions must be answered.");

                }


                var alreadyAnsweredQuestions = unitOfWork.Surveys.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(nextCategory.ID, model.SurveyInstanceId);



                var viewModel = new SurveyPageViewModel
                {
                    SurveyInstanceId = model.SurveyInstanceId,
                    PendingSurveyId = model.PendingSurveyId,
                    StatusGuid = theSurvey.StatusGuid ?? Guid.NewGuid(),
                    Category = CategoryViewModel.Convert(nextCategory),
                    Questions = alreadyAnsweredQuestions.Select(x => new QuestionAnswerViewModel { Question = QuestionViewModel.Convert(x.Item1), Answer = new AnswerViewModel { ResponseNum = x?.Item2?.ResponseNum ?? null } }).ToList()
                };
                ModelState.Remove("Category.Id");
                ModelState.Remove("BackOnePage");

                return View("SurveyPage", viewModel);
            }
        }

        [HttpPost]
        public ActionResult GoBackSurvey(SurveyPageViewModel model)
        {
            return SurveyPage(model);
        }

        [HttpGet]
        [ActionName("GoBackSurvey")]
        //[Route("Survey/SurveyPage/{SurveyInstanceId}/{pendingSurveyId}/{statGuid}")]
        public ActionResult SurveyPage(int SurveyInstanceId, Guid penSurveyId, Guid statGuid)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var theSurvey = unitOfWork.Surveys.LockAndGetSurvey(penSurveyId, statGuid);

                if (theSurvey == null) return RedirectToAction("SurveyLocked");

                int surveyInstanceId = SurveyInstanceId;

                var surveyInstance = unitOfWork.Surveys.GetSurveyInstanceByIdSYSTEM(surveyInstanceId);



                var nextCategory = unitOfWork.Surveys.GetLastCategory(surveyInstance.SurveyID);

                if (nextCategory == null)
                {
                    throw new Exception();
                }

                var alreadyAnsweredQuestions = unitOfWork.Surveys.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(nextCategory.ID, surveyInstanceId);



                var viewModel = new SurveyPageViewModel
                {
                    SurveyInstanceId = surveyInstanceId,
                    PendingSurveyId = penSurveyId,
                    StatusGuid = theSurvey.StatusGuid ?? Guid.NewGuid(),
                    Category = CategoryViewModel.Convert(nextCategory),
                    Questions = alreadyAnsweredQuestions.Select(x => new QuestionAnswerViewModel { Question = QuestionViewModel.Convert(x.Item1), Answer = new AnswerViewModel { ResponseNum = x?.Item2?.ResponseNum ?? null } }).ToList()
                };
                ModelState.Remove("Category.Id");
                ModelState.Remove("BackOnePage");

                //return View("../Articles/Index", viewModel);
                return View("SurveyPage", viewModel);
            }
        }

        public ActionResult EndSurvey(int? SurveyInstanceId, Guid penSurveyId, Guid statGuid)
        {
            return View();
        }


        public ActionResult SurveyLocked()
        {

            return View();
        }

        public ActionResult SurveyMissingItems(int? SurveyInstanceId, Guid penSurveyId, Guid statGuid)
        {

            return View();
        }

        public ActionResult SurveyError(int? SurveyInstanceId, Guid penSurveyId, Guid statGuid)
        {

            return View();
        }

        public ActionResult SurveyDone()
        {

            return View();
        }


        public ActionResult SaveSurvey(int? SurveyInstanceId, Guid penSurveyId, Guid statGuid)
        {
            if (SurveyInstanceId == null)
            {
                throw new Exception();
            }

            try
            {
                using (var unitOfWork = new UnitOfWork())
                {



                    var result = unitOfWork.Surveys.FinishSurvey(SurveyInstanceId ?? -1, statGuid);

                    if (result)
                    {
                        unitOfWork.Complete();
                        return RedirectToAction("SurveyDone");
                    }
                    else
                    {
                        return RedirectToAction("SurveyMissingItems", new { SurveyInstanceId = SurveyInstanceId, penSurveyId = penSurveyId, statGuid = statGuid });
                    }


                }

            }
            catch (Exception e)
            {
                return RedirectToAction("SurveyError", new { SurveyInstanceId = SurveyInstanceId, penSurveyId = penSurveyId, statGuid = statGuid });
            }
        }


    }
}