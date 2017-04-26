using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using EmployeeEvaluationSystem.Entity;
using System.Collections.Generic;
using EmployeeEvaluationSystem.MVC.Controllers;
using System.Web.Mvc;
using System.Linq;
using EmployeeEvaluationSystem.MVC.Models;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;

namespace EmployeeEvaluationSystem.Tests.Controllers
{
    [TestClass]
    public class SurveyControllerTest
    {
        [TestMethod]
        public void StartSurveyGuestWithValidEmailReturnsView()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123"}

            });

            var unitOfWork = new UnitOfWork(x.GetContext());

            var ctrl = new SurveyController(null, null, x.GetCreator());

            var result = ctrl.StartSurvey(guid, "123");

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
        }

        [TestMethod]
        public void StartSurveyGuestWithInvalidEmailReturnsRedirect()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123"}

            });

            var ctrl = new SurveyController(null, null, x.GetCreator());

            var result = ctrl.StartSurvey(guid, "456");

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);
            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "SurveyNoPermission")
                );
        }
    }
}
