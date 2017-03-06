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
    public class PermissionsController : Controller
    {
        // GET: Permissions
        public ActionResult Index()
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var unconvertedPermissions = unitOfWork.Permissions.GetAllPermissions(userId).ToList();

                var convertedPermissions = unconvertedPermissions?.Select(x => PersonalPermissionViewModel.Convert(x))?.ToList();

                return View(convertedPermissions);
            }
        }

        // GET: Permissions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var unconvertedPermission = unitOfWork.Permissions.GetPermission(userId, id);

                var convertedPermission = PersonalPermissionViewModel.Convert(unconvertedPermission);

                if (convertedPermission == null)
                {
                    return HttpNotFound();
                }

                return View(convertedPermission);
            }
        }

        // GET: Permissions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Permissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PersonalPermissionViewModel model)
        {
            var userId = User?.Identity?.GetUserId();

            var permission = new Permission()
            {
                ID = model.ID,
                Name = model.Name,
                Add = model.Add,
                Edit = model.Edit,
                Delete = model.Delete,
                View = model.View
            };

            using (var unitOfWork = new UnitOfWork())
            {
                
                unitOfWork.Permissions.AddPermissionToDb(userId, permission);

                unitOfWork.Complete();

                return RedirectToAction("Index");
            }
        }

        // GET: Permissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var unconvertedPermission = unitOfWork.Permissions.GetPermission(userId, id);

                var convertedPermission = PersonalPermissionViewModel.Convert(unconvertedPermission);

                if (convertedPermission == null)
                {
                    return HttpNotFound();
                }

                return View(convertedPermission);
            }
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonalPermissionViewModel model)
        {
            var userId = User?.Identity?.GetUserId();

            var permission = new Permission()
            {
                ID = model.ID,
                Name = model.Name,
                Add = model.Add,
                Edit = model.Edit,
                Delete = model.Delete,
                View = model.View
            };

            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.Permissions.EditPermission(userId, permission);

                unitOfWork.Complete();

                return RedirectToAction("Index");
            }
        }

        // GET: Permissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var unconvertedPermission = unitOfWork.Permissions.GetPermission(userId, id);

                var convertedPermission = PersonalPermissionViewModel.Convert(unconvertedPermission);

                if (convertedPermission == null)
                {
                    return HttpNotFound();
                }

                return View(convertedPermission);
            }
        }

        // POST: Permissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.Permissions.DeletePermission(userId, id);

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
