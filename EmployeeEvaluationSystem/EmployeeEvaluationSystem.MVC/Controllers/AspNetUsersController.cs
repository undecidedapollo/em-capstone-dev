using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using Microsoft.AspNet.Identity;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class AspNetUsersController : Controller
    {

        [Authorize]
        // GET: AspNetUsers
        public async Task<ActionResult> Index()
        {
            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                var unconvertedUsers = unitOfWork.Users.GetAllUsers(userId).ToList();



                var convertedUsers = unconvertedUsers?.Select(x => PersonalAspNetUserViewModel.Convert(x))?.ToList();

                return View(convertedUsers);
            }

            
        }

        // GET: AspNetUsers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                AspNetUser aspNetUser = unitOfWork.Users.GetUser(userId, id);

                if (aspNetUser == null)
                {
                    return HttpNotFound();
                }

                var returnUser = PersonalAspNetUserViewModel.Convert(aspNetUser);

                return View(returnUser);
            }



          
        }

        // GET: AspNetUsers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                AspNetUser aspNetUser = unitOfWork.Users.GetUser(userId, id);

                if (aspNetUser == null)
                {
                    return HttpNotFound();
                }

                var returnUser = PersonalAspNetUserViewModel.Convert(aspNetUser);

                return View(returnUser);
            }
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email,PhoneNumber,FirstName,LastName,MailingAddress")] PersonalAspNetUserViewModel aspNetUser)
        {

            if (aspNetUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {

                var newUser = unitOfWork.Users.EditUser(userId, aspNetUser);

                unitOfWork.Complete();

                return RedirectToAction("Index");
            }

        }

        // GET: AspNetUsers/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                AspNetUser aspNetUser = unitOfWork.Users.GetUser(userId, id);

                if (aspNetUser == null)
                {
                    return HttpNotFound();
                }

                var returnUser = PersonalAspNetUserViewModel.Convert(aspNetUser);

                return View(returnUser);
            }
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.Users.DeleteUser(userId, id);

                unitOfWork.Complete();

                return RedirectToAction("Index");
            }
            
        }
    }
}
