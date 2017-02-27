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
    public class CohortPermissionsController : Controller
    {
        // GET: CohortPermissions
        public ActionResult Index()
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var unconvertedCohortPermissions = unitOfWork.CohortPermissions.GetAllCohortPermissions(userId).ToList();

                var convertedCohortPermissions = unconvertedCohortPermissions?.Select(x =>  PersonalCohortPermissionViewModel.Convert(x))?.ToList();

                return View(convertedCohortPermissions);
            }
        }

        // GET: CohortPermissions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var unconvertedCohortPermission = unitOfWork.CohortPermissions.GetCohortPermission(userId, id);

                var convertedCohortPermission = PersonalCohortPermissionViewModel.Convert(unconvertedCohortPermission);

                if (convertedCohortPermission == null)
                {
                    return HttpNotFound();
                }

                return View(convertedCohortPermission);
            }
        }

        // GET: CohortPermissions/Create
        public ActionResult Create()
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {

                var permissions =
                    unitOfWork.Permissions.GetAllPermissions(userId)?
                        .Select(x => PersonalPermissionViewModel.Convert(x))?.ToList();

                ViewBag.Cohort = new SelectList(permissions, "ID", "Name");
                ViewBag.User = new SelectList(permissions, "ID", "Name");
                ViewBag.Survey = new SelectList(permissions, "ID", "Name");
                ViewBag.Report = new SelectList(unitOfWork.Permissions.GetAllPermissions(userId).ToList(), "ID", "Name");


                
                return View();
            }
        }

        // POST: CohortPermissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PersonalCohortPermissionViewModel model)
        {
            var cohortPermission = new CohortPermission()
            {
                ID = model.ID,
                Name = model.Name,
                Cohort = model.Cohort,
                User = model.User,
                Survey = model.Survey,
                Report = model.Report
            };

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.CohortPermissions.AddCohortPermissionToDb(userId, cohortPermission);

                unitOfWork.Complete();

                return RedirectToAction("Index");
            }
        }

        // GET: CohortPermissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var permissions = unitOfWork.Permissions.GetAllPermissions(userId);

                ViewBag.Cohort = new SelectList(permissions, "ID", "Name");
                ViewBag.User = new SelectList(permissions, "ID", "Name");
                ViewBag.Survey = new SelectList(permissions, "ID", "Name");
                ViewBag.Report = new SelectList(permissions, "ID", "Name");

                var unconvertedCohortPermission = unitOfWork.CohortPermissions.GetCohortPermission(userId, id);

                var convertedCohortPermission = PersonalCohortPermissionViewModel.Convert(unconvertedCohortPermission);

                return View(convertedCohortPermission);
            }
        }

        // POST: CohortPermissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonalCohortPermissionViewModel model)
        {
            var userId = User?.Identity?.GetUserId();

            var cohortPermission = new CohortPermission()
            {
                ID = model.ID,
                Name = model.Name,
                Cohort = model.Cohort,
                User = model.User,
                Survey = model.Survey,
                Report = model.Report
            };

            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.CohortPermissions.EditCohortPermission(userId, cohortPermission);

                unitOfWork.Complete();

                return RedirectToAction("Index");
            }
        }

        // GET: CohortPermissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var unconvertedCohortPermission = unitOfWork.CohortPermissions.GetCohortPermission(userId, id);

                var convertedCohortPermission = PersonalCohortPermissionViewModel.Convert(unconvertedCohortPermission);

                return View(convertedCohortPermission);
            }
        }

        // POST: CohortPermissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.CohortPermissions.DeleteCohortPermission(userId, id);

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
