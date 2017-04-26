using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.MVC.Models;
using Microsoft.AspNet.Identity;
using EmployeeEvaluationSystem.SharedObjects.Enums;
using EmployeeEvaluationSystem.MVC.Models.Survey;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Survey;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;

namespace EmployeeEvaluationSystem.MVC.Controllers
{


    [Authorize(Roles = "Admin")]
    public class CohortsController : Controller
    {
        private HttpRequestBase passedInRequest;

        private IUnitOfWorkCreator creator;

        public IUnitOfWorkCreator Creator
        {
            get { return creator ?? HttpContext.GetOwinContext().Get<IUnitOfWorkCreator>(); }
            private set { creator = value; }
        }

        public CohortsController()
        {
        }

        public CohortsController(IUnitOfWorkCreator creator, HttpRequestBase request = null)
        {
            this.creator = creator;
            this.passedInRequest = request;
        }

<<<<<<< HEAD
=======




>>>>>>> a3bcaf5df315eceb597a24d03ddcf6e29fb284ae
        // GET: Cohort
        public ActionResult Index(int? id)
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = this.Creator.Create())
            {
                var unconvertedCohorts = unitOfWork.Cohorts.GetAllCohorts(userId).Where(x => x.IsDeleted == false).ToList();

                var convertedCohorts = unconvertedCohorts?.Select(x => PersonalCohortViewModel.Convert(x))?.ToList();

                var viewModel = new CohortIndexViewModel();

                viewModel.Cohorts = convertedCohorts;

                if (id != null)
                {
                    ViewBag.CohortID = id.Value;

                    var cohortUsers = unitOfWork.CohortUsers.GetAllCohortUsers(userId).Where(i => i.CohortID == id.Value);

                    var users = new List<PersonalAspNetUserViewModel>();

                    foreach (CohortUser cohortUser in cohortUsers)
                    {
                        var user = unitOfWork.Users.GetUser(userId, cohortUser.UserID);

                        users.Add(PersonalAspNetUserViewModel.Convert(user));
                    }

                    viewModel.Users = users;
                }

                return View(viewModel);
            }
        }

        // GET: Cohort/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = this.Creator.Create())
            {
                Cohort cohort = unitOfWork.Cohorts.GetCohort(userId, id);

                if (cohort == null)
                {
                    return HttpNotFound();
                }

                var surveys = unitOfWork.Surveys.GetAllOfferedSurveysForCohort(userId, id.Value);

                var outgoingSurveys = surveys.Where(x => x.IsDeleted == false).Select(x => new SurveysViewModel
                {
                    Id = x.ID,
                    DateClosed = x.DateClosed,
                    DateOpened = x.DateOpen,
                    SurveyName = x.Survey.Name,
                    SurveyType = x.SurveyType.Name
                }).ToList();

                var theModel = new CohortDetailsViewmodel
                {
                    Surveys = outgoingSurveys,
                    TheCohort = cohort
                };

                return View(theModel);
            }
        }

        // GET: Cohort/Create
        public ActionResult Create()
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = this.Creator.Create())
            {
                var unconvertedUsers = unitOfWork.Cohorts.GetAllUsersThatAreNotPartOfACohort(userId).ToList();

                var convertedUsers = unconvertedUsers?.Select(x => PersonalAspNetUserViewModel.Convert(x))?.ToList();

                var viewModel = new CreateCohortViewModel()
                {
                    Users = convertedUsers,
                    Cohort = new PersonalCohortViewModel()
                };

                return View(viewModel);
            }
        }

        // POST: Cohort/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateCohortViewModel model, List<string> ids)
        {
            var userId = User?.Identity?.GetUserId();

            var shouldReturn = false;

            if(model?.Cohort.Name == null)
            {
                ModelState.AddModelError("", "You must add a name to be able to create a cohort.");
                shouldReturn = true;
            }
            else if (model?.Cohort?.Description == null)
            {
                ModelState.AddModelError("", "You must add a description to be able to create a cohort.");
                shouldReturn = true;

            }else if(ids == null || ids.Count < 1)
            {
                ModelState.AddModelError("", "You must add at least one user to be able to create a cohort.");
                shouldReturn = true;
            }

            if (shouldReturn)
            {
                using (var unitOfWork = this.Creator.Create())
                {
                    var unconvertedUsers = unitOfWork.Cohorts.GetAllUsersThatAreNotPartOfACohort(userId).ToList();

                    var convertedUsers = unconvertedUsers?.Select(x => PersonalAspNetUserViewModel.Convert(x))?.ToList();

                    var viewModel = new CreateCohortViewModel()
                    {
                        Users = convertedUsers,
                        Cohort = model?.Cohort ?? new PersonalCohortViewModel()
                    };

                    return View(viewModel);
                }
            }


            var usersToRegister = new List<string>();

            using (var unitOfWork = this.Creator.Create())
            {
                var cohort = new Cohort()
                {
                    Name = model.Cohort.Name,
                    Description = model.Cohort.Description,
                    DateCreated = DateTime.UtcNow
                };
                
                foreach (var id in ids)
                {
                    var user = PersonalAspNetUserViewModel.Convert(unitOfWork.Users.GetUser(userId, id));

                    if(user.EmailConfirmed == false)
                    {
                        usersToRegister.Add(id);
                    }

                    var cohortUser = new CohortUser()
                    {
                        UserID = id
                    };

                    //unitOfWork.CohortUsers.AddCohortUserToDb(userId, cohortUser);

                    cohort.CohortUsers.Add(cohortUser);
                }

                unitOfWork.Cohorts.AddCohortToDb(userId, cohort);

                unitOfWork.Complete();
            }

            TempData["usersToRegister"] = usersToRegister.ToList();

            return RedirectToAction("SendEmailConfirmationTokenAsync", "Account", new { subject = "Confirm Email" });
        }

        // GET: Cohort/StartEvaluation/5
        public ActionResult StartEvaluation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = this.Creator.Create())
            {
                var surveys = unitOfWork.Surveys.GetAllSurveys(userId);

                var surveyTypes = unitOfWork.Surveys.GetAllSurveyTypes(userId);

                var roleTypes = unitOfWork.Surveys.GetUserSurveyRoles();

                var cohort = unitOfWork.Cohorts.GetCohort(userId, id);

                var assignedSurveys = new List<(Survey, SurveyType, StartEvaluationViewModel.SurveyState)?>();

                var newSurvList = new List<CSSurveyViewModel>();

                foreach(var surv in surveys)
                {

                    var newSurvModel = new CSSurveyViewModel
                    {
                        TheSurvey = surv
                    };

                    newSurvList.Add(newSurvModel);

                    var item = cohort.SurveysAvailables.Where(x => x.SurveyID == surv.ID && x.IsDeleted == false).OrderByDescending(X => X.ID).FirstOrDefault();


                    if(item == null)
                    {
                        newSurvModel.TheSurveyType = surveyTypes.FirstOrDefault(x => x.ID == 1);
                        newSurvModel.TheState = StartEvaluationViewModel.SurveyState.AVAILABLE;
                    }
                    else
                    {

                        var nextResult = unitOfWork.Surveys.GetNextAvailableSurveyTypeForSurveyInCohort(surv.ID, cohort.ID);

                        if (nextResult != null)
                        {
                            newSurvModel.TheState = StartEvaluationViewModel.SurveyState.AVAILABLE;
                            newSurvModel.TheSurveyType = nextResult;

                        }
                        else
                        {
                            newSurvModel.TheState = StartEvaluationViewModel.SurveyState.IN_PROGRESS;
                            newSurvModel.TheSurveyType = item.SurveyType;
                        }
                    }
                }

                var model = new StartEvaluationViewModel()
                {
                    Surveys = surveys,
                    SurveyTypes = surveyTypes,
                    RoleQuantities = roleTypes.Select(x => new RaterQuantityViewModel {
                        Id = x.ID,
                        DisplayName = x.Name,
                        Quantity = x.ID == Convert.ToInt32(SurveyRoleEnum.SELF) ? 1 : 0,
                        CanChange = x.ID == Convert.ToInt32(SurveyRoleEnum.SELF) ? false : true }
                    ).ToList(),
                    AssignedSurveys = assignedSurveys ?? throw new Exception(),
                    NewSurveys = newSurvList,
                    CohortID = cohort.ID,
                    DateClosed = (DateTime.UtcNow + TimeSpan.FromDays(30)).Date,
                    DateOpen = DateTime.UtcNow.Date
                };

                return View(model);
            }
        }

        [HttpPost]
        // POST: Cohort/StartEvaluation/5
        public async Task<ActionResult> StartEvaluation(StartEvaluationViewModel model)
        {
            var roleModels = new List<CreateAvailableSurveyRolesModel>();


            if(model.DateClosed <= model.DateOpen)
            {
                ModelState.AddModelError("", "Your end date must be before the start date.");
                return View(model);
            }


            foreach(var item in model.RoleQuantities)
            {
                if(item == null)
                {
                    continue;
                }


                if(item.Quantity < 0)
                {
                    ModelState.AddModelError("", "All quantities must be zero or greater.");
                    return View(model);
                }

                if (item.Quantity == 0)
                {
                    continue;
                }

                if(item.Id == 1)
                {
                    continue;
                }

                var roleModel = new CreateAvailableSurveyRolesModel()
                {
                    RoleId = item.Id,
                    Quantity = item.Quantity
                };

                roleModels.Add(roleModel);
            }
            var surveyModel = new CreateAvailableSurveyModel()
            {
                CohortId = model.CohortID,
                SurveyId = model.SurveyID,
                DateStart = model.DateOpen,
                DateEnd = model.DateClosed,
                SurveyTypeId = model.SurveyTypeID,
                RolesSurveyFor = roleModels
            };

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = this.Creator.Create())
            {
                var availableSurvey = unitOfWork.Surveys.CreateAnAvailableSurveyForCohort(userId, surveyModel);

                unitOfWork.Complete();

                availableSurvey = unitOfWork.Surveys.GetAnAvailableSurveyForCohort(userId, availableSurvey.ID, false);

                foreach (var item in availableSurvey.PendingSurveys)
                {
                    var scheme = Request?.Url?.Scheme ?? passedInRequest.Url.Scheme;
                    var hostname = Request?.Url?.Host ?? passedInRequest.Url.Host;

                    if ((Request?.Url?.Port ?? passedInRequest.Url.Port) != 80 || (Request?.Url?.Port ?? passedInRequest.Url.Port) != 43)
                    {
                        hostname += ":" + (Request?.Url?.Port ?? passedInRequest.Url.Port);
                    }

                    var theUrl = $"{scheme}://{hostname}/Survey/StartSurvey?pendingSurveyId={item.Id}&userId={item.UserTakenById}";

                    await SendgridEmailService.GetInstance().SendAsync(
                        new IdentityMessage
                        {
                            Destination = item.UserTakenBy.Email,
                            Subject = $"You have a new survey available.",
                            Body = $"There is a pending survey waiting for you. The survey is called \"{availableSurvey.Survey.Name}\" - {availableSurvey.SurveyType.Name}. The survey is available from {availableSurvey.DateOpen} to {availableSurvey.DateClosed}. Please click the link to take the survey: <a href=\"" + theUrl + "\">Survey</a>"
                        });
                }

                return View("AssignedSurveyEmailSent");
            } 
        }


        // GET: Cohort/Edit/5
        [HttpGet]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = this.Creator.Create())
            {
                var newCohort = unitOfWork.Cohorts.GetCohort(userId, id.Value);

                return View(newCohort);
            }
        }

        // POST: Cohort/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,IsDeleted,DateDeleted,DateCreated")] Cohort cohort)
        {
            if (cohort == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = this.Creator.Create())
            {
                var newCohort = unitOfWork.Cohorts.EditCohort(userId, cohort);

                unitOfWork.Complete();

                return RedirectToAction("Index");
            }
        }

        // GET: Cohort/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = this.Creator.Create())
            {
                Cohort cohort = unitOfWork.Cohorts.GetCohort(userId, id);

                if (cohort == null)
                {
                    return HttpNotFound();
                }

                return View(cohort);
            }
        }

        
        // POST: Cohort/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = this.Creator.Create())
            {
                var result = unitOfWork.Cohorts.DeleteCohort(userId, id);

                if(result == false)
                {
                    ModelState.AddModelError("", "You cannot delete this cohort because it has already started evaluations.");
                    Cohort cohort = unitOfWork.Cohorts.GetCohort(userId, id);

                    if (cohort == null)
                    {
                        return HttpNotFound();
                    }

                    return View(cohort);
                }


                unitOfWork.Complete();

                return RedirectToAction("Index");
            }
        }
    }
}
