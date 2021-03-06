﻿using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Helpers.Locks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Survey;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.MVC.Infrastructure.Hangfire;
using EmployeeEvaluationSystem.MVC.Models.Survey;
using EmployeeEvaluationSystem.SharedObjects.Enums;
using EmployeeEvaluationSystem.SharedObjects.Exceptions.Lock;
using EmployeeEvaluationSystem.SharedObjects.Extensions;
using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private IUnitOfWorkCreator creator;

        public IUnitOfWorkCreator Creator
        {
            get {
                return creator ?? HttpContext.GetOwinContext().Get<IUnitOfWorkCreator>();
            }
            private set { creator = value; }
        }

        public SurveyController()
        {
        }

        public SurveyController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, IUnitOfWorkCreator creator, HttpRequestBase request = null)
        {
            this.passedInRequest = request;
            UserManager = userManager;
            this.creator = creator;
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
                    using (var unitOfWork = this.Creator.Create())
                    {
                        var auth = unitOfWork.Surveys.CanExistingUserTakeSurvey(userId, pendingSurveyId);

                        if (!auth) throw new UnauthorizedAccessException();
                    }

                    var user = this.UserManager.FindById(userId);
                    if(user == null)
                    {
                        throw new UnauthorizedAccessException();
                    }
                        
                        
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

            using (var unitOfWork = this.Creator.Create())
            {
                bool canTake = guestMode ? unitOfWork.Surveys.CanGuestUserTakeSurvey(email, pendingSurveyId) : unitOfWork.Surveys.CanExistingUserTakeSurvey(userId, pendingSurveyId);

                if (!canTake)
                {
                    return RedirectToAction("SurveyNoPermission");
                }
            }

            return View();
        }

        public ActionResult ContinueSurvey()
        {
            return View();
        }

        public ActionResult SurveyMissingData()
        {
            return View();
        }

        //[Route("Survey/SurveyPage/{pendingSurveyId}/{email?}")]
        public ActionResult SurveyPage(Guid pendingSurveyId, string email = null)
        {
            var userId = User?.Identity?.GetUserId();


            if (userId == null && email == null)
            {
                return RedirectToAction("SurveyMissingData");
            }

            bool guestMode = userId == null;

            using (var unitOfWork = this.Creator.Create())
            {
                bool canTake = guestMode ? unitOfWork.Surveys.CanGuestUserTakeSurvey(email, pendingSurveyId) : unitOfWork.Surveys.CanExistingUserTakeSurvey(userId, pendingSurveyId);

                if (!canTake)
                {
                    return RedirectToAction("SurveyNoPermission");
                }


                var lockPendingSurvey = unitOfWork.Surveys.LockAndGetSurvey(pendingSurveyId);

                if (lockPendingSurvey == null)
                {
                    return RedirectToAction("SurveyLocked");
                }

                if (lockPendingSurvey.StatusGuid == null)
                {
                    return RedirectToAction("SurveyError", new { });
                }

                var dbPendingSurvey = unitOfWork.Surveys.GetPendingSurveySYSTEM(pendingSurveyId).ThrowIfNull();

                if(dbPendingSurvey == null)
                {
                    return RedirectToAction("SurveyError", new {  });
                }

                if(dbPendingSurvey?.SurveysAvailable == null)
                {
                    return RedirectToAction("SurveyError", new { });
                }

                if(DateTime.UtcNow < dbPendingSurvey.SurveysAvailable.DateOpen)
                {
                    return RedirectToAction("SurveyTooSoon", new { DateOpens = dbPendingSurvey.SurveysAvailable.DateOpen });
                }

                if (DateTime.UtcNow > dbPendingSurvey.SurveysAvailable.DateClosed)
                {
                    return RedirectToAction("SurveyTooLate");
                }


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

                    theInstance = unitOfWork.Surveys.GetSurveyInstanceByIdSYSTEM(theInstance.ID) ?? theInstance; //Need to get categories.
                }
                else if (instanceSurvey.DateFinished == null)
                {
                    theInstance = instanceSurvey;
                }
                else
                {
                    return RedirectToAction("SurveyDone");
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

        public ActionResult SurveyTooSoon(DateTime DateOpens)
        {
            return View(new QuickModel { DateOpens = DateOpens });
        }

        public ActionResult SurveyTooLate()
        {
            return View();
        }

        public ActionResult SurveyNoPermission()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SurveyPage(SurveyPageViewModel model)
        {
            using (var unitOfWork = this.Creator.Create())
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
            using (var unitOfWork = this.Creator.Create())
            {
                var theSurvey = unitOfWork.Surveys.LockAndGetSurvey(penSurveyId, statGuid);

                if (theSurvey == null) return RedirectToAction("SurveyLocked");

                int surveyInstanceId = SurveyInstanceId;

                var surveyInstance = unitOfWork.Surveys.GetSurveyInstanceByIdSYSTEM(surveyInstanceId);



                var nextCategory = unitOfWork.Surveys.GetLastCategory(surveyInstance.SurveyID);

                if (nextCategory == null)
                {
                    return RedirectToAction("SurveyError", new { SurveyInstanceId = SurveyInstanceId, penSurveyId = penSurveyId, statGuid = statGuid });
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
                return RedirectToAction("SurveyMissingData");
            }

            try
            {
                using (var unitOfWork = this.Creator.Create())
                {



                    var result = unitOfWork.Surveys.FinishSurvey(SurveyInstanceId ?? -1, statGuid);

                    if (result)
                    {
                        unitOfWork.Complete();

                        

                        var pendingSurver = unitOfWork.Surveys.GetPendingSurveySYSTEM(penSurveyId);

                        if(pendingSurver?.SurveysAvailable != null)
                        {
                            BackgroundJob.Enqueue(() => SurveyHelper.CheckAndMarkSurveyFinished(pendingSurver.SurveysAvailable.CohortID, pendingSurver.SurveysAvailable.ID));
                        }

                        

                        if (pendingSurver == null || pendingSurver.UserTakenById == null || pendingSurver.UserTakenById != userId)
                        {
                            return RedirectToAction("SurveyDone");
                        }
                        else
                        {
                            return RedirectToAction("ChooseRaters", new { penSurveyId = penSurveyId });
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

            using (var unitOfWork = this.Creator.Create())
            {

                var pendingSurvey = unitOfWork.Surveys.GetPendingSurvey(userId, penSurveyId);

                if (pendingSurvey.UserSurveyForId != userId && pendingSurvey.UserSurveyRoleID != Convert.ToInt32(SurveyRoleEnum.SELF))
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

                foreach (var rater in theRaters)
                {


                    var canChange = true;
                    var resendEmail = true;
                    var status = "Not Started";

                    if(rater.UserSurveyRoleID == Convert.ToInt32(SurveyRoleEnum.SELF))
                    {
                        continue;
                    }


                    if (rater.SurveyInstance != null)
                    {
                        canChange = false;
                        if (rater.SurveyInstance.DateFinished != null)
                        {
                            status = "Survey Finished";
                            resendEmail = false;
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
                        Id = rater.Id,
                        CanResendEmail = resendEmail,
                        RaterFirstName = rater.RaterFirstName,
                        RaterLastName = rater.RaterLastName
                    };

                    viewModel.Raters.Add(newRater);
                }

                var previousRaters = unitOfWork.Surveys.GetMostRecentRatersForUser(userId, 10).ToList();

                foreach (var aRater in expectedRaters)
                {

                    if (aRater.UserSurveyRoleId == Convert.ToInt32(SurveyRoleEnum.SELF))
                    {
                        continue;
                    }

                    var count = viewModel.Raters.Count(x => x.RoleId == aRater.UserSurveyRoleId);

                    if (count < aRater.Quantity)
                    {
                        var diff = aRater.Quantity - count;

                        

                        for (var i = 0; i < diff; i++)
                        {
                            if(previousRaters.Any(x => x.RoleId == aRater.UserSurveyRoleId))
                            {
                                var possibleRater = previousRaters.FirstOrDefault(x => x.RoleId == aRater.UserSurveyRoleId);
                                var newRater = new RaterViewModel
                                {
                                    CanChange = true,
                                    Email = possibleRater.email,
                                    RaterFirstName = possibleRater.firstName,
                                    RaterLastName = possibleRater.lastName,
                                    Role = aRater.UserSurveyRole.Name,
                                    RoleId = aRater.UserSurveyRole.ID,
                                    Status = "New Rater",
                                    CanResendEmail = false
                                };

                                previousRaters.Remove(possibleRater);
                                viewModel.Raters.Add(newRater);
                            }
                            else
                            {
                                var newRater = new RaterViewModel
                                {
                                    CanChange = true,
                                    Email = null,
                                    Role = aRater.UserSurveyRole.Name,
                                    RoleId = aRater.UserSurveyRole.ID,
                                    Status = "New Rater",
                                    CanResendEmail = false
                                };

                                viewModel.Raters.Add(newRater);
                            }

                            

                          
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

            using (var unitOfWork = this.Creator.Create())
            {

                var pendingSurvey = unitOfWork.Surveys.GetPendingSurvey(userId, theModel.PendingSurveyId);

                var currentUser = unitOfWork.Users.GetUser(userId, userId);

                if (pendingSurvey.UserSurveyForId != userId && pendingSurvey.UserSurveyRoleID != Convert.ToInt32(SurveyRoleEnum.SELF))
                {
                    throw new UnauthorizedAccessException();
                }

                SurveysAvailable surveyAvailable = unitOfWork.Surveys.GetAnAvailableSurveyForCohort(userId, pendingSurvey.SurveyAvailToMeID);

                var expectedRaters = surveyAvailable.SurveysAvailableToes;


                var theRaters = unitOfWork.Surveys.GetPendingSurveysOfRatersForUser(userId, theModel.PendingSurveyId)?.Where(x => x.UserTakenById != userId).ToList();




                var visitedRaters = new List<PendingSurvey>();
                var ratersToAdd = new List<PendingSurvey>();
                var ratersToRemove = new List<PendingSurvey>();

                theModel.Raters.ForEach(x =>
                {
                    x.Email = x?.Email?.Trim()?.ToLower();
                });


                var hasDuplicates = theModel.Raters.GroupBy(x => x.Email).Select(x => x.Count()).Any(x => x > 1);

                if (hasDuplicates)
                {
                    return RedirectToAction("ChooseRaters", new { penSurveyId = theModel.PendingSurveyId, error = "All emails must be unique" });
                }

                var isSameEmailAsUser = theModel.Raters.Any(x => x.Email == currentUser.Email);

                if (isSameEmailAsUser)
                {
                    return RedirectToAction("ChooseRaters", new { penSurveyId = theModel.PendingSurveyId, error = "You cannot use the same email as your own. Please put in other user's emails." });
                }
                foreach (var rater in theModel.Raters)
                {
                    


                    var realRater = theRaters.FirstOrDefault(x => x.Id == rater.Id);

                    if (realRater == null)
                    {
                        if (rater?.Email == null)
                        {
                            continue;
                        }

                        var newRater = new PendingSurvey
                        {
                            Id = Guid.NewGuid(),
                            StatusId = 1,
                            DateSent = DateTime.UtcNow,
                            Email = rater.Email,
                            IsDeleted = false,
                            SurveyAvailToMeID = pendingSurvey.SurveyAvailToMeID,
                            UserSurveyForId = pendingSurvey.UserSurveyForId,
                            UserSurveyRoleID = rater.RoleId,
                            RaterFirstName = rater.RaterFirstName,
                            RaterLastName = rater.RaterLastName
                        };

                        ratersToAdd.Add(newRater);
                    }
                    else
                    {
                        if (realRater.Email == rater.Email)
                        {
                            visitedRaters.Add(realRater);
                            continue;
                        }

                        if (realRater.SurveyInstance != null)
                        {
                            visitedRaters.Add(realRater);
                            continue;
                        }

                        ratersToRemove.Add(realRater);

                        if(rater?.Email == null)
                        {
                            continue;
                        }

                        var newRater = new PendingSurvey
                        {
                            Id = Guid.NewGuid(),
                            StatusId = 1,
                            DateSent = DateTime.UtcNow,
                            Email = rater.Email,
                            IsDeleted = false,
                            SurveyAvailToMeID = pendingSurvey.SurveyAvailToMeID,
                            UserSurveyForId = pendingSurvey.UserSurveyForId,
                            UserSurveyRoleID = rater.RoleId,
                            RaterFirstName = rater.RaterFirstName,
                            RaterLastName = rater.RaterLastName
                        };

                        ratersToAdd.Add(newRater);
                    }
                }

                var toRemove = theRaters.Except(visitedRaters).ToList();

                toRemove.AddRange(ratersToRemove);
                unitOfWork.Surveys.TryRemovePendingSurveysSYSTEM(toRemove.Distinct().ToList());
                unitOfWork.Surveys.TryToAddPendingSurveysSYSTEM(ratersToAdd);

                foreach (var rater in expectedRaters)
                {
                    var count = visitedRaters.Count(x => x.UserSurveyRoleID == rater.UserSurveyRoleId) + ratersToAdd.Count(x => x.UserSurveyRoleID == rater.UserSurveyRoleId);

                    if (count > rater.Quantity)
                    {
                        throw new Exception("There are too many raters of a particular type.");
                    }
                }

                unitOfWork.Complete();


                foreach (var rater in ratersToAdd)
                {
                    var scheme = Request?.Url?.Scheme ?? passedInRequest.Url.Scheme;
                    var hostname = Request?.Url?.Host ?? passedInRequest.Url.Host;

                    if ((Request?.Url?.Port ?? passedInRequest.Url.Port) != 80 || (Request?.Url?.Port ?? passedInRequest.Url.Port) != 43)
                    {
                        hostname += ":" + (Request?.Url?.Port ?? passedInRequest.Url.Port);
                    }

                    string theUrl = "";

                    if(rater.UserTakenBy != null)
                    {
                        theUrl = $"{scheme}://{hostname}/Survey/StartSurvey?pendingSurveyId={rater.Id}&userId={rater.UserTakenById}";
                    }
                    else
                    {
                        theUrl = $"{scheme}://{hostname}/Survey/StartSurvey?pendingSurveyId={rater.Id}&email={rater.Email}";
                    }

                    

                    await SendgridEmailService.GetInstance().SendAsync(
                        new IdentityMessage
                        {
                            Destination = rater.Email,
                            Subject = $"Employee Survey, regarding {currentUser.FirstName + " " + currentUser.LastName}",
                            Body = $"Hello {rater?.UserTakenBy?.FirstName ?? rater.RaterFirstName} {rater?.UserTakenBy?.LastName ?? rater.RaterLastName}, There is a pending survey waiting for you to take regarding {currentUser.FirstName + " " + currentUser.LastName}. The survey is called \"{pendingSurvey.SurveysAvailable.Survey.Name}\" - {pendingSurvey.SurveysAvailable.SurveyType.Name}. The survey is available from {pendingSurvey.SurveysAvailable.DateOpen} to {pendingSurvey.SurveysAvailable.DateClosed}. Please click the link to take the survey: <a href=\"" + theUrl + "\">Survey</a>"
                        });
                }

                return RedirectToAction("Index", "UserHub");
            }
        }

        [Authorize]
        public async Task<ActionResult> ResendEmail(Guid id)
        {
            var userId = User?.Identity?.GetUserId() ?? throw new UnauthorizedAccessException();

            using (var unitOfWork = this.Creator.Create())
            {

                var pendingSurvey = unitOfWork.Surveys.GetPendingSurvey(userId, id);

                if(pendingSurvey == null || (pendingSurvey.Email == null && pendingSurvey.UserTakenBy == null) || pendingSurvey?.SurveyInstance?.DateFinished != null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var userSurveyFor = pendingSurvey.UserSurveyFor ?? throw new Exception();

                var scheme = Request?.Url?.Scheme ?? passedInRequest.Url.Scheme;
                var hostname = Request?.Url?.Host ?? passedInRequest.Url.Host;

                if ((Request?.Url?.Port ?? passedInRequest.Url.Port) != 80 || (Request?.Url?.Port ?? passedInRequest.Url.Port) != 43)
                {
                    hostname += ":" + (Request?.Url?.Port ?? passedInRequest.Url.Port);
                }

                string theUrl = "";

                if (pendingSurvey.UserTakenBy != null)
                {
                    theUrl = $"{scheme}://{hostname}/Survey/StartSurvey?pendingSurveyId={pendingSurvey.Id}&userId={pendingSurvey.UserTakenById}";
                }
                else
                {
                    theUrl = $"{scheme}://{hostname}/Survey/StartSurvey?pendingSurveyId={pendingSurvey.Id}&email={pendingSurvey.Email}";
                }

                await SendgridEmailService.GetInstance().SendAsync(
                    new IdentityMessage
                    {
                        Destination = pendingSurvey?.UserTakenBy?.Email ?? pendingSurvey.Email,
                        Subject = $"Employee Survey, regarding {userSurveyFor.FirstName + " " + userSurveyFor.LastName}",
                        Body = $"Hello {pendingSurvey?.UserTakenBy?.FirstName ?? pendingSurvey.RaterFirstName} {pendingSurvey?.UserTakenBy?.LastName ?? pendingSurvey.RaterLastName}, There is a pending survey waiting for you to take regarding {userSurveyFor.FirstName + " " + userSurveyFor.LastName}. The survey is called \"{pendingSurvey.SurveysAvailable.Survey.Name}\" - {pendingSurvey.SurveysAvailable.SurveyType.Name}. The survey is available from {pendingSurvey.SurveysAvailable.DateOpen} to {pendingSurvey.SurveysAvailable.DateClosed}. Please click the link to take the survey: <a href=\"" + theUrl + "\">Survey</a>"
                    });
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Test(int surveyId, int cohortId)
        {
            using (var unitOfWork = this.Creator.Create())
            {
                var nextSuveyType = unitOfWork.Surveys.GetNextAvailableSurveyTypeForSurveyInCohort(surveyId, cohortId);

                return null;
            }
        }

        public ActionResult ViewSurveyAnswers(Guid pendingSurveyId, int? categoryId)
        {
            var userId = User?.Identity?.GetUserId() ?? throw new UnauthorizedAccessException();

            var isAdmin = User.IsInRole("Admin");

            using (var unitOfWork = this.Creator.Create())
            {
                ViewBag.PendingSurveyID = pendingSurveyId;
                var survey = unitOfWork.Surveys.GetPendingSurvey(userId, pendingSurveyId);

                if(survey == null)
                {
                    throw new Exception();
                }

                if(isAdmin || survey.UserTakenById == userId)
                {
                    var data = survey.SurveyInstance ?? throw new Exception();

                }
                else
                {
                    throw new UnauthorizedAccessException();
                }

                if(survey.SurveyInstance == null)
                {
                    throw new Exception();
                }

                var viewModel = new ShowSurveyDetailsViewModel()
                {
                    UserEmail = survey?.UserTakenBy?.Email ?? survey.Email,
                    UserName = $"{survey?.UserTakenBy?.FirstName?? survey?.RaterFirstName ?? "Unknown" } {survey?.UserTakenBy?.LastName ?? survey?.RaterLastName ?? "Unknown"}"  ,
                    UserForEmail = survey.UserSurveyFor.Email,
                    UserForName = $"{survey?.UserSurveyFor?.FirstName} {survey?.UserSurveyFor?.LastName}",
                    DateCompleted = survey.SurveyInstance.DateFinished ?? throw new Exception(),
                    DateStarted = survey.SurveyInstance.DateStarted,
                    SurveyName = survey.SurveyInstance.Survey.Name,
                    SurveyType = survey.SurveysAvailable.SurveyType.Name,
                    UserRole = survey.UserSurveyRole.Name,
                    SurvAvailId = survey.SurveysAvailable.ID
                };

                var surveyInstanceID = survey.SurveyInstanceID;

                var categories = survey.SurveysAvailable.Survey.Categories;

                viewModel.Categories = categories;

                if (categoryId != null)
                {
                    ViewBag.CategoryID = categoryId.Value;

                    viewModel.Questions = unitOfWork.Surveys.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(categoryId.Value, surveyInstanceID.Value);
                }

                return View(viewModel);
            }
        }

        // GET: Survey/SurveyDelete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult SurveyDelete(int? id)
        {
            var userId = User?.Identity?.GetUserId() ?? throw new UnauthorizedAccessException();

            using (var unitOfWork = this.Creator.Create())
            {
                var surv = unitOfWork.Surveys.GetAnAvailableSurveyForCohort(userId, id.Value);

                if(surv == null)
                {
                    return HttpNotFound();
                }

                return View(new SurveysViewModel { DateClosed = surv.DateClosed, DateOpened = surv.DateOpen, SurveyName = surv.Survey.Name, SurveyType = surv.SurveyType.Name });
            }
        }

        // POST: Survey/SurveyDelete/5
        [HttpPost, ActionName("SurveyDelete")]
        [Authorize(Roles = "Admin")]
        public ActionResult SurveyDeleteConfirmed(int? id)
        {
            var userId = User?.Identity?.GetUserId() ?? throw new UnauthorizedAccessException();

            try
            {
                using (var unitOfWork = this.Creator.Create())
                {
                    var surv = unitOfWork.Surveys.GetAnAvailableSurveyForCohort(userId, id.Value);
                    var result = unitOfWork.Surveys.DeleteSurveyAvailable(userId, id.Value);

                    if (!result)
                    {
                        ModelState.AddModelError("", "You cannot delete a survey if a user has already started taking the survey.");

                        if (surv == null)
                        {
                            return HttpNotFound();
                        }

                        return View(new SurveysViewModel { DateClosed = surv.DateClosed, DateOpened = surv.DateOpen, SurveyName = surv.Survey.Name, SurveyType = surv.SurveyType.Name });
                    }

                    unitOfWork.Complete();

                    return RedirectToAction("Details", "Cohorts", new { id = surv.CohortID });
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occured trying to delete your survey. Please try again!");

                return RedirectToAction("Index", "Cohorts");
            }
           
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SurveyDetails(int surveyId)
        {
            var userId = User?.Identity?.GetUserId() ?? throw new UnauthorizedAccessException();

            using (var unitOfWork = this.Creator.Create())
            {
                var surveysAvailable = unitOfWork.Surveys.GetAnAvailableSurveyForCohort(userId, surveyId);


                var cohortUsers = unitOfWork.Cohorts.GetCohort(userId, surveysAvailable.CohortID)?.CohortUsers;

                if (surveysAvailable == null || cohortUsers == null)
                {
                    return new HttpNotFoundResult();
                }

                var viewModel = new SurveyDetailsViewModel
                {
                    TheSurvey = new SurveysViewModel
                    {
                        Id = surveysAvailable.ID,
                        DateClosed = surveysAvailable.DateClosed,
                        DateOpened = surveysAvailable.DateOpen,
                        SurveyName = surveysAvailable.Survey.Name,
                        SurveyType = surveysAvailable.SurveyType.Name,
                        IsFinished = surveysAvailable.IsCompleted
                    },
                    UserGroups = new List<UserGroupSurveyStatus>()
                };


                foreach(var user in cohortUsers)
                {
                    var newModel = new UserGroupSurveyStatus
                    {
                        Name = user.AspNetUser.FirstName + " " + user.AspNetUser.LastName,
                        UserId = user.UserID,
                        UsersForSurvey = new List<UserSurveyStatus>()
                    };


                    var userSurveys = unitOfWork.Surveys.GetPendingSurveysOfRatersForUser(user.UserID, surveysAvailable.ID);

                    foreach(var surv in userSurveys)
                    {
                        var isStarted = surv.SurveyInstance?.DateStarted != null;
                        var isFinished = surv.SurveyInstance?.DateFinished != null;

                        var status = isFinished ? "Completed" : isStarted ? "Started" : "Not Started";

                        var NameOrEmail = surv.Email;

                        if(surv?.UserTakenBy?.FirstName != null)
                        {
                            NameOrEmail = surv?.UserTakenBy?.FirstName + " " + surv?.UserTakenBy?.LastName;
                        }

                        if(surv?.UserSurveyRoleID == 1 && isFinished)
                        {
                            newModel.CanShowReport = true;
                        }


                        var newItem = new UserSurveyStatus
                        {
                            CanViewResults = isFinished,
                            DateStarted = surv?.SurveyInstance?.DateStarted,
                            DateFinished = surv?.SurveyInstance?.DateFinished,
                            Id = surv.Id,
                            Status = status,
                            NameOrEmail = NameOrEmail,
                            RoleName = surv.UserSurveyRole.Name,
                            RoleId = surv.UserSurveyRole.ID,
                            CanResendEmail = isFinished ? false : true
                        };

                        newModel.UsersForSurvey.Add(newItem);
                    }

                    

                    foreach(var roles in surveysAvailable.SurveysAvailableToes)
                    {
                        var count = newModel.UsersForSurvey.Count(x => x.RoleId == roles.UserSurveyRole.ID);

                        if(count < roles.Quantity)
                        {
                            var diff = roles.Quantity - count;

                            for(var i = 0; i < diff; i++)
                            {
                                newModel.UsersForSurvey.Add(new UserSurveyStatus
                                {
                                    Id = null,
                                    DateFinished = null,
                                    DateStarted = null,
                                    CanViewResults = false,
                                    NameOrEmail = "Not Assigned To Rater",
                                    RoleId = roles.UserSurveyRole.ID,
                                    RoleName = roles.UserSurveyRole.Name,
                                    Status = "Not Assigned To Rater",
                                    CanResendEmail = false
                                });
                            }
                        }
                    }

                    

                    viewModel.UserGroups.Add(newModel);
                }

                if(viewModel.UserGroups.Any(x => x.CanShowReport))
                {
                    viewModel.CanShowAllReport = true;
                }

                return View(viewModel);
            }
        }

        [Authorize]
        public async Task<ActionResult> MarkFinished(int id)
        {
            var userId = User?.Identity?.GetUserId() ?? throw new UnauthorizedAccessException();

            using (var unitOfWork = this.Creator.Create())
            {
                unitOfWork.Surveys.TryMarkAsFinished(id);
                unitOfWork.Complete();
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

    }
}