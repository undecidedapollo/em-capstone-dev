using EmployeeEvaluationSystem.MVC.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmployeeEvaluationSystem.MVC.Controllers.Tests
{
    [TestClass()]
    public class UserHubControllerTest
    {
        [TestMethod()]
        public void UserHubIndexNotNullPendingSurveyTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey {SurveyAvailToMeID = 2},
                    new PendingSurvey {UserSurveyRoleID = 1}

                });

            var ctrl = new UserHubController(x.GetCreator());

            var result = ctrl.Index(null, 2);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<UserHubIndexViewModel>(routeResult.Model);
            Assert.IsNotNull(modelResult?.PendingSurveys);

            UTH.IsTrue(
                modelResult.PendingSurveys.Any(y => y.UserSurveyRoleID == 1),
                modelResult.PendingSurveys.Any(y => y.SurveyAvailToMeID == 2)
            );
        }
        
    }
}



