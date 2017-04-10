using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Helpers.Locks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Survey;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.MVC.Models.Survey;
using EmployeeEvaluationSystem.SharedObjects.Enums;
using EmployeeEvaluationSystem.SharedObjects.Exceptions.Lock;
using EmployeeEvaluationSystem.SharedObjects.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                else if (userId != null && email == null)
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

            var userId = User?.Identity?.GetUserId();

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

                        var pendingSurver = unitOfWork.Surveys.GetPendingSurveySYSTEM(penSurveyId);

                        if(pendingSurver == null || pendingSurver.UserTakenById == null || pendingSurver.UserTakenById != userId)
                        {
                            return RedirectToAction("SurveyDone");
                        }
                        else
                        {
                            return RedirectToAction("ChooseRaters", new { penSurveyId=penSurveyId });
                        }



                        
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

        [Authorize]
        [HttpGet]
        public ActionResult ChooseRaters(Guid penSurveyId, string error = null)
        {
            var userId = User?.Identity?.GetUserId() ?? throw new UnauthorizedAccessException();

            using (var unitOfWork = new UnitOfWork())
            {

                var pendingSurvey = unitOfWork.Surveys.GetPendingSurvey(userId, penSurveyId);

                if(pendingSurvey.UserSurveyForId != userId && pendingSurvey.UserSurveyRoleID != Convert.ToInt32(SurveyRoleEnum.SELF))
                {
                    throw new UnauthorizedAccessException();
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    ModelState.AddModelError("", error);
                }

                SurveysAvailable surveyAvailable = unitOfWork.Surveys.GetAnAvailableSurveyForCohort(userId, pendingSurvey.SurveyAvailToMeID);

                var expectedRaters = surveyAvailable.SurveysAvailableToes;


                var theRaters = unitOfWork.Surveys.GetPendingSurveysOfRatersForUser(userId, penSurveyId);



                var viewModel = new RatersPageViewModel
                {
                    PendingSurveyId = penSurveyId
                };

                foreach(var rater in theRaters)
                {

                    var canChange = true;
                    var status = "Not Started";

                    if(rater.SurveyInstance != null)
                    {
                        canChange = false;
                        if (rater.SurveyInstance.DateFinished != null)
                        {
                            status = "Survey Finished";
                        }
                        else
                        {
                            status = "Survey Started";
                        }

                    }

                    var newRater = new RaterViewModel()
                    {
                        Email = rater.Email,
                        Role = rater.UserSurveyRole.Name,
                        RoleId = rater.UserSurveyRole.ID,
                        Status = status,
                        CanChange = canChange,
                        Id = rater.Id
                    };

                    viewModel.Raters.Add(newRater);
                }

                foreach(var aRater in expectedRaters)
                {

                    if(aRater.UserSurveyRoleId == Convert.ToInt32(SurveyRoleEnum.SELF))
                    {
                        continue;
                    }

                    var count = viewModel.Raters.Count(x => x.RoleId == aRater.UserSurveyRoleId);

                    if(count < aRater.Quantity)
                    {
                        var diff = aRater.Quantity - count;

                        for(var i = 0; i < diff; i++)
                        {
                            var newRater = new RaterViewModel
                            {
                                CanChange = true,
                                Email = null,
                                Role = aRater.UserSurveyRole.Name,
                                RoleId = aRater.UserSurveyRole.ID,
                                Status = "New Rater"
                            };

                            viewModel.Raters.Add(newRater);
                        }
                    }
                }

                return View(viewModel);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ChooseRaters(RatersPageViewModel theModel)
        {
            var userId = User?.Identity?.GetUserId() ?? throw new UnauthorizedAccessException();

            using (var unitOfWork = new UnitOfWork())
            {

                var pendingSurvey = unitOfWork.Surveys.GetPendingSurvey(userId, theModel.PendingSurveyId);

                var currentUser = unitOfWork.Users.GetUser(userId, userId);

                if (pendingSurvey.UserSurveyForId != userId && pendingSurvey.UserSurveyRoleID != Convert.ToInt32(SurveyRoleEnum.SELF))
                {
                    throw new UnauthorizedAccessException();
                }

                SurveysAvailable surveyAvailable = unitOfWork.Surveys.GetAnAvailableSurveyForCohort(userId, pendingSurvey.SurveyAvailToMeID);

                var expectedRaters = surveyAvailable.SurveysAvailableToes;


                var theRaters = unitOfWork.Surveys.GetPendingSurveysOfRatersForUser(userId, theModel.PendingSurveyId);

                


                var visitedRaters = new List<PendingSurvey>();
                var ratersToAdd = new List<PendingSurvey>();
                var ratersToRemove = new List<PendingSurvey>();

                theModel.Raters.ForEach(x =>
                {
                    x.Email = x.Email.Trim().ToLower();
                });


                var hasDuplicates = theModel.Raters.GroupBy(x => x.Email).Select(x => x.Count()).Any(x => x > 1);

                if (hasDuplicates)
                {
                    return RedirectToAction("ChooseRaters", new { penSurveyId = theModel.PendingSurveyId, error= "All emails must be unique" });
                }

                var isSameEmailAsUser = theModel.Raters.Any(x => x.Email == currentUser.Email);

                if (isSameEmailAsUser)
                {
                    return RedirectToAction("ChooseRaters", new { penSurveyId = theModel.PendingSurveyId, error= "You cannot use the same email as your own. Please put in other user's emails." });
                }
                foreach (var rater in theModel.Raters)
                {
                    var realRater = theRaters.FirstOrDefault(x => x.Id == rater.Id);

                    if (realRater == null)
                    {
                        var newRater = new PendingSurvey
                        {
                            Id = Guid.NewGuid(),
                            StatusId = 1,
                            DateSent = DateTime.UtcNow,
                            Email = rater.Email,
                            IsDeleted = false,
                            SurveyAvailToMeID = pendingSurvey.SurveyAvailToMeID,
                            UserSurveyForId = pendingSurvey.UserSurveyForId,
                            UserSurveyRoleID = rater.RoleId
                        };

                        ratersToAdd.Add(newRater);
                    }
                    else
                    {
                        if(realRater.Email == rater.Email)
                        {
                            visitedRaters.Add(realRater);
                            continue;
                        }

                        if(realRater.SurveyInstance != null)
                        {
                            visitedRaters.Add(realRater);
                            continue;
                        }

                        ratersToRemove.Add(realRater);

                        var newRater = new PendingSurvey
                        {
                            Id = Guid.NewGuid(),
                            StatusId = 1,
                            DateSent = DateTime.UtcNow,
                            Email = rater.Email,
                            IsDeleted = false,
                            SurveyAvailToMeID = pendingSurvey.SurveyAvailToMeID,
                            UserSurveyForId = pendingSurvey.UserSurveyForId,
                            UserSurveyRoleID = rater.RoleId
                        };

                        ratersToAdd.Add(newRater);
                    }
                }

                var toRemove = theRaters.Except(visitedRaters).ToList();

                toRemove.AddRange(ratersToRemove);
                unitOfWork.Surveys.TryRemovePendingSurveysSYSTEM(toRemove.Distinct().ToList());
                unitOfWork.Surveys.TryToAddPendingSurveysSYSTEM(ratersToAdd);

                foreach(var rater in expectedRaters)
                {
                    var count = visitedRaters.Count(x => x.UserSurveyRoleID == rater.UserSurveyRoleId) + ratersToAdd.Count(x => x.UserSurveyRoleID == rater.UserSurveyRoleId);

                    if(count > rater.Quantity)
                    {
                        throw new Exception("There are too many raters of a particular type.");
                    }
                }

                unitOfWork.Complete();


                foreach(var rater in ratersToAdd)
                {
                    var scheme = Request?.Url?.Scheme ?? passedInRequest.Url.Scheme;
                    var hostname = Request?.Url?.Host ?? passedInRequest.Url.Host;

                    if ((Request?.Url?.Port ?? passedInRequest.Url.Port) != 80 || (Request?.Url?.Port ?? passedInRequest.Url.Port) != 43)
                    {
                        hostname += ":" + (Request?.Url?.Port ?? passedInRequest.Url.Port);
                    }

                    var theUrl = $"{scheme}://{hostname}/Survey/StartSurvey?pendingSurveyId={rater.Id}&email={rater.Email}";

                    await SendgridEmailService.GetInstance().SendAsync(
                        new IdentityMessage {
                            Destination = rater.Email,
                            Subject = $"Employee Survey, regarding {currentUser.FirstName + " " + currentUser.LastName}",
                            Body = $"There is a pending survey waiting for you to take regarding {currentUser.FirstName + " " + currentUser.LastName}. Please click the link to take the survey: <a href=\"" + theUrl + "\">Survey</a>"
                        });
                }

                return RedirectToAction("ChooseRaters", new { penSurveyId = theModel.PendingSurveyId });
            }
        }


        public ActionResult Test(int surveyId, int cohortId)
        {
            using(var unitOfWork = new UnitOfWork())
            {
                var nextSuveyType = unitOfWork.Surveys.GetNextAvailableSurveyTypeForSurveyInCohort(surveyId, cohortId);

                return null;
            }
        }

    }
}