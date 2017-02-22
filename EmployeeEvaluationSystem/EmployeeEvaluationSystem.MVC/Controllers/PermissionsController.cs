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
    public class PermissionsController : Controller
    {
        // GET: Permissions
        public ActionResult Index()
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var permissions = unitOfWork.Permissions.GetAllPermissions(userId);

                return View(permissions);
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
                var permission = unitOfWork.Permissions.GetPermission(userId, id);

                if (permission == null)
                {
                    return HttpNotFound();
                }

                return View(permission);
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
        public ActionResult Create([Bind(Include = "ID,Name,Add,Edit,Delete,View")] Permission permission)
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.Permissions.AddPermissionToDb(userId, permission);

                    unitOfWork.Complete();

                    return RedirectToAction("Index");
                }

                return View(permission);
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
                var permission = unitOfWork.Permissions.GetPermission(userId, id);

                if (permission == null)
                {
                    return HttpNotFound();
                }

                return View(permission);
            }
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Add,Edit,Delete,View")] Permission permission)
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.Permissions.EditPermission(userId, permission);

                    unitOfWork.Complete();

                    return RedirectToAction("Index");
                }

                return View(permission);
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
                var permission = unitOfWork.Permissions.GetPermission(userId, id);

                if (permission == null)
                {
                    return HttpNotFound();
                }

                return View(permission);
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
