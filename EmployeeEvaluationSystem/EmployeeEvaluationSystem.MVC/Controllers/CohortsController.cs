using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
using Microsoft.AspNet.Identity.Owin;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class CohortsController : Controller
    {
        // GET: Cohort
        public ActionResult Index(int? id)
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var unconvertedCohorts = unitOfWork.Cohorts.GetAllCohorts(userId).ToList();

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

            using (var unitOfWork = new UnitOfWork())
            {
                Cohort cohort = unitOfWork.Cohorts.GetCohort(userId, id);


                if (cohort == null)
                {
                    return HttpNotFound();
                }

                return View(cohort);
            }
        }

        // GET: Cohort/Create
        public ActionResult Create()
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
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
            var usersToRegister = new List<string>();

            using (var unitOfWork = new UnitOfWork())
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
                        UserID = id,
                        CohortPermissionId = 0
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

        // GET: Cohort/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                Cohort cohort = unitOfWork.Cohorts.GetCohort(userId, id);

                if (cohort == null)
                {
                    return HttpNotFound();
                }

                return View(cohort);
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

            using (var unitOfWork = new UnitOfWork())
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

            using (var unitOfWork = new UnitOfWork())
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

            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.Cohorts.DeleteCohort(userId, id);

                unitOfWork.Complete();

                return RedirectToAction("Index");
            }
        }

        /*
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        */
    }
}
