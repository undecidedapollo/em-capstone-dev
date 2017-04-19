using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity;
using Microsoft.AspNet.Identity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using EmployeeEvaluationSystem.MVC.Models;
using System.Data;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class ReportGenerationController : Controller
    {
        private EmployeeDatabaseEntities entities;

        // GET: ReportGeneration
        public ActionResult Index()
        {                    
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string ReportHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader stReader = new StringReader(ReportHtml);
                Document pdfReport = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfReport, stream);
                pdfReport.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfReport, stReader);
                pdfReport.Close();
                return File(stream.ToArray(), "application/pdf", "EvaluationReport.pdf");
            }
        }

        // GET: ReportGeneration/Create
        public ActionResult Create()
        {

            var userId = User?.Identity?.GetUserId();

            using (var unitOfWork = new UnitOfWork())
            {
                

                var viewModel = new ReportGenerationViewModel()
                {
                   
                };

                return View(viewModel);
            }
        }

        // POST: ReportGeneration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReportGenerationViewModel model, List<string> ids)
        {
            var userId = User?.Identity?.GetUserId();
            var usersToRegister = new List<string>();

            using (var unitOfWork = new UnitOfWork())
            {
                

                foreach (var id in ids)
                {
                    var user = PersonalAspNetUserViewModel.Convert(unitOfWork.Users.GetUser(userId, id));

                    

                    var cohortUser = new CohortUser()
                    {
                        UserID = id
                    };

                    
                }

                

                unitOfWork.Complete();
            }

            TempData["usersToRegister"] = usersToRegister.ToList();

            return RedirectToAction("SendEmailConfirmationTokenAsync", "Account", new { subject = "Confirm Email" });
        }


        public int GetRating(int id)
        {
            AnswerInstance answer = new AnswerInstance();
            var rating = 0;
            if (answer.ID == id)
            {
                rating = answer.ResponseNum;
            }
            return rating;
        }

        public double CalculateAverage(int id)
        {
            //The id is equivalent to the role value
            DataTable table = new DataTable();
            //AnswerInstance answerValue = new AnswerInstance();
                        
            var rating = this.GetRating(id);
           
            return rating;
        }

        // GET: Report  
        public ActionResult ReportDetail()
        {
            var unitOfWork = new UnitOfWork();
            var dbcontext = new EmployeeDatabaseEntities();
            ReportRepository objDet = new ReportRepository(unitOfWork, dbcontext);
            ReportDetails reportData = new ReportDetails();
            ReportRole reportRole = new ReportRole();
            List<ReportRole> masterData = objDet.GetDetailsForReport("2", 2).ToList();

            //reportData.EmpAvgRatings = masterData[0].EmpAvgRatings;
            //reportData.UserRole = masterData[0].UserRole;
            //reportData.MasterDetails = masterData[0].Questions;

            return View("Index");
        }


    }
}