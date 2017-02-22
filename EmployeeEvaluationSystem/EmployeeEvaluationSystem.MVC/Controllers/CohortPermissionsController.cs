using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity;
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
                var cohortPermissions = unitOfWork.CohortPermissions.GetAllCohortPermissions(userId);

                return View(cohortPermissions);
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
                var cohortPermission = unitOfWork.CohortPermissions.GetCohortPermission(userId, id);

                if (cohortPermission == null)
                {
                    return HttpNotFound();
                }

                return View(cohortPermission);
            }
        }

        // GET: CohortPermissions/Create
        public ActionResult Create()
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                //Uncomment the following if you want the user to select permission level. Otherwise permission is set to read on everything.
                /*
                var permissions = unitOfWork.Permissions.GetAllPermissions(userId);

                ViewBag.Cohort = new SelectList(permissions, "ID", "Name");
                ViewBag.User = new SelectList(permissions, "ID", "Name");
                ViewBag.Survey = new SelectList(permissions, "ID", "Name");
                ViewBag.Report = new SelectList(permissions, "ID", "Name");
                */

                return View();
            }
        }

        // POST: CohortPermissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Cohort,User,Survey,Report")] CohortPermission cohortPermission)
        {
            cohortPermission.Cohort = 1;
            cohortPermission.User = 1;
            cohortPermission.Survey = 1;
            cohortPermission.Report = 1;

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.CohortPermissions.AddCohortPermissionToDb(userId, cohortPermission);

                    unitOfWork.Complete();

                    return RedirectToAction("Index");
                }

                //Uncomment the following if you want the user to select permission level. Otherwise permission is set to read on everything.
                /*
                var permissions = unitOfWork.Permissions.GetAllPermissions(userId);

                ViewBag.Cohort = new SelectList(permissions, "ID", "Name");
                ViewBag.User = new SelectList(permissions, "ID", "Name");
                ViewBag.Survey = new SelectList(permissions, "ID", "Name");
                ViewBag.Report = new SelectList(permissions, "ID", "Name");
                */

                return View();
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
                //Uncomment the following if you want the user to select permission level. Otherwise permission is set to read on everything.
                /*
                var permissions = unitOfWork.Permissions.GetAllPermissions(userId);

                ViewBag.Cohort = new SelectList(permissions, "ID", "Name");
                ViewBag.User = new SelectList(permissions, "ID", "Name");
                ViewBag.Survey = new SelectList(permissions, "ID", "Name");
                ViewBag.Report = new SelectList(permissions, "ID", "Name");
                */

                var cohortPermission = unitOfWork.CohortPermissions.GetCohortPermission(userId, id);

                return View(cohortPermission);
            }
        }

        // POST: CohortPermissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Cohort,User,Survey,Report")] CohortPermission cohortPermission)
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.CohortPermissions.EditCohortPermission(userId, cohortPermission);

                    unitOfWork.Complete();

                    return RedirectToAction("Index");
                }

                //Uncomment the following if you want the user to select permission level. Otherwise permission is set to read on everything.
                /*
                var permissions = unitOfWork.Permissions.GetAllPermissions(userId);

                ViewBag.Cohort = new SelectList(permissions, "ID", "Name");
                ViewBag.User = new SelectList(permissions, "ID", "Name");
                ViewBag.Survey = new SelectList(permissions, "ID", "Name");
                ViewBag.Report = new SelectList(permissions, "ID", "Name");
                */

                return View(cohortPermission);
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
                var cohortPermission = unitOfWork.CohortPermissions.GetCohortPermission(userId, id);

                return View(cohortPermission);
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
