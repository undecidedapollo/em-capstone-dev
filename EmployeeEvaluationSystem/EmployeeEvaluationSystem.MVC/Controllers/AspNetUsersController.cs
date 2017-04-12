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
using System.IO;
using LumenWorks.Framework.IO.Csv;
using EmployeeEvaluationSystem.MVC.Models;
using Microsoft.AspNet.Identity.Owin;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
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

        [Authorize]
        [HttpGet]
        // GET: AspNetUsers
        public async Task<ActionResult> AddCSV()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        // POST: AspNetUsers
        public async Task<ActionResult> AddCSV(HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (upload != null && upload.ContentLength > 0)
                    {

                        if (upload.FileName.EndsWith(".csv"))
                        {
                            Stream stream = upload.InputStream;
                            DataTable csvTable = new DataTable();
                            using (CsvReader csvReader =
                                new CsvReader(new StreamReader(stream), true))
                            {
                                csvTable.Load(csvReader);
                            }

                            if(csvTable.Columns.Count != 6)
                            {
                                ModelState.AddModelError("File", "The CSV file is required to have six fields for columns.");
                                return View();
                            }

                            int rowNum = 0;
                            bool isError = false;
                            var usersToRegister = new List<CSVEmployeeViewModel>();


                            foreach(DataRow row in csvTable.Rows)
                            {
                                rowNum++;
                                if (row == null)
                                {
                                    ModelState.AddModelError("File", "ERROR ON ROW {rowNum}: There was an unknown error on this row. Please make sure that it is in the correct format and that there are no errors.");
                                    return View();
                                }

                                if(row?.ItemArray?.Length != 6)
                                {
                                    ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: The CSV file is required to have six fields for every row.");
                                    return View();
                                }

                                var empId = row[0] as string;
                                var firstName = row[1] as string;
                                var lastName = row[2] as string;
                                var email = row[3] as string;
                                var mailingaddress = row[4] as string;
                                var phone = row[5] as string;

                                if (empId == null)
                                {
                                    ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: An employee id is required");
                                    isError = true;
                                }

                                if (firstName == null)
                                {
                                    ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: A first name is required");
                                    isError = true;
                                }

                                if (lastName == null)
                                {
                                    ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: A last name is required");
                                    isError = true;
                                }

                                if (email == null)
                                {
                                    ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: An email is required");
                                    isError = true;
                                }

                                if (mailingaddress == null)
                                {
                                    ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: A mailing address is required");
                                    isError = true;
                                }

                                if (phone == null)
                                {
                                    ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: A phone number is required");
                                    isError = true;
                                }

                                if (isError)
                                {
                                    return View("AddCSV");
                                }


                                usersToRegister.Add(new CSVEmployeeViewModel
                                {
                                    EmployeeId = empId,
                                    FirstName = firstName,
                                    LastName = lastName,
                                    Email = email,
                                    PhoneNumber = phone,
                                    MailingAddress = mailingaddress
                                });
                            }


                            return View(usersToRegister);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("File", "There was an error processing your CSV. Please make sure that it is in the correct format and that there are no errors.");
            }

            return View();

        }


        [Authorize]
        [HttpPost]
        // POST: AspNetUsers
        public async Task<ActionResult> SaveCSV(IEnumerable<CSVEmployeeViewModel> table)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    int rowNum = 0;

                    var isError = false;

                    var usersToRegister = new List<RegisterViewModel>();

                    foreach(var row in table)
                    {
                        if(row == null)
                        {
                            continue;
                        }

                        if(string.IsNullOrWhiteSpace(row.EmployeeId))
                        {
                            ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: An employee id is required");
                            isError = true;
                        }

                        if(string.IsNullOrWhiteSpace(row.FirstName))
                        {
                            ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: A first name is required");
                            isError = true;
                        }

                        if (string.IsNullOrWhiteSpace(row.LastName))
                        {
                            ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: A last name is required");
                            isError = true;
                        }

                        if (string.IsNullOrWhiteSpace(row.Email))
                        {
                            ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: An email is required");
                            isError = true;
                        }

                        if (string.IsNullOrWhiteSpace(row.MailingAddress))
                        {
                            ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: A mailing address is required");
                            isError = true;
                        }

                        if (string.IsNullOrWhiteSpace(row.PhoneNumber))
                        {
                            ModelState.AddModelError("File", $"ERROR ON ROW {rowNum}: A phone number is required");
                            isError = true;
                        }

                        if (isError)
                        {
                            return View("AddCSV");
                        }


                        usersToRegister.Add(row.Convert());
                    }

                    var um = HttpContext.GetOwinContext().Get<ApplicationUserManager>();
                    var sm = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();


                    var accController = new AccountController(um, sm, this.Request);

                    var result = await accController.RegisterMultipleUsers(usersToRegister);

                    if (result.Successful)
                    {
                        return View();
                    }else
                    {
                        var badUser = result.FailedUser;

                        if(badUser == null)
                        {
                            ModelState.AddModelError("File", $"ERROR: An unknown error has occured while trying to register your users. Please try again.");
                        }
                        else
                        {
                            var index = usersToRegister.IndexOf(badUser);

                            ModelState.AddModelError("File", $"ERROR ON ROW {index}: There was an error trying to register this user. This can happen for numerous reasons such as an existing email, invalid data, etc. Please check your data and try again.");
                        }

                        return View("AddCSV");
                    }

                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("File", "There was an error processing your CSV. Please try again");
            }

            return View("AddCSV");

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
