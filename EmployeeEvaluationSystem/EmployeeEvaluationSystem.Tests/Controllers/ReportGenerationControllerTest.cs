using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.MVC.Controllers;
using EmployeeEvaluationSystem.MVC.Models;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Moq;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories;


namespace EmployeeEvaluationSystem.Tests.Controllers
{
    [TestClass]
    public class ReportGenerationControllerTest
    {       

        [TestMethod]
        public void ReportGenerationIndexReturnsViewResult()
        {
            ReportGenerationController reportController = new ReportGenerationController();
            ActionResult result = reportController.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void ReportGenerationIndexWillReturnDynamicViewResult()
        {
            ReportGenerationController reportController = new ReportGenerationController();
            dynamic result = reportController.Index();
            Assert.AreEqual("", result.ViewName);
        }

        [TestMethod]
        public void ReportGenerationControllerIsNotNull()
        {   
            var ctrl = new ReportGenerationController();            
            var result = ctrl.Index();
           
            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            Assert.IsNotNull(routeResult);           
        }

        [TestMethod]
        public void ReportGenerationControllerUserId()
        {
            var guid = Guid.NewGuid();
            var userId = "1a";
            int surveyAvail = 1;
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123", SurveyAvailToMeID = surveyAvail, IsDeleted = false, UserSurveyForId = userId}

            });
            
            var ctrl = new ReportGenerationController(x.GetCreator());
            var result = ctrl.Index();

            //var model = new ReportDetailsViewModel
            //{
            //    ResponseItems = {QuestionId = 1},
            //    Categories = {CategoryId = 1, Name = "Test Category"},
            //    Header = "Title One",
            //    Header2 = "Title Two"
                
            //};


            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                //var page = unitOfWork.Reports.GetDetailsForReport(userId, surveyAvail);
            }

               
            
            //var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            //Assert.IsNotNull(routeResult);
        }
    }
}

