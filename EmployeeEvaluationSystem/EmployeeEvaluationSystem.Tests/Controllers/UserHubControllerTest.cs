using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
            var viewResult = UTH.CheckIsTypeNotNullAndGetObj<UserHubIndexViewModel>(modelResult);
            Assert.IsNotNull(modelResult?.PendingSurveys);

            UTH.IsTrue(
                modelResult.PendingSurveys.Any(y => y.UserSurveyRoleID == 1),
                modelResult.PendingSurveys.Any(y => y.SurveyAvailToMeID == 2)
            );
        }

        [TestMethod()]
        public void UserHubIndexPendingSurveyIsNullTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey {SurveyInstanceID = null},
                    new PendingSurvey {Email = null}

                });

            var ctrl = new UserHubController(x.GetCreator());

            var result = ctrl.Index(null, null);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<UserHubIndexViewModel>(routeResult.Model);
            Assert.IsNotNull(modelResult?.PendingSurveys);

            UTH.IsTrue(
                modelResult.PendingSurveys.Any(y => y.SurveyInstanceID == null),
                modelResult.PendingSurveys.Any(y => y.Email == null)
            );
        }

        [TestMethod()]
        public void UserHubIndexSurveyNotDeletedTest()
        {
                          
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey { SurveyInstanceID = 1},
                    new PendingSurvey { IsDeleted = false},
                    new PendingSurvey { StatusId = 1}

                });

            var ctrl = new UserHubController(x.GetCreator());

            var result = ctrl.Index(null, 1);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<UserHubIndexViewModel>(routeResult.Model);
            Assert.IsNotNull(modelResult?.PendingSurveys);
            

            UTH.IsTrue(             
                modelResult.PendingSurveys.Any(y => y.IsDeleted == false),
                modelResult.PendingSurveys.Any(y => y.SurveyInstanceID == 1),
                modelResult.PendingSurveys.Any(y => y.StatusId == 1)
            );
        }

        [TestMethod()]
        public void UserHubIndexSurveyIsDeletedTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey { SurveyInstanceID = 1},
                    new PendingSurvey { IsDeleted = true}              

                });

            var ctrl = new UserHubController(x.GetCreator());

            var result = ctrl.Index(null, 1);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<UserHubIndexViewModel>(routeResult.Model);
            Assert.IsNotNull(modelResult?.PendingSurveys);


            UTH.IsTrue(                
                !modelResult.PendingSurveys.Any(y => y.IsDeleted == true),
                modelResult.PendingSurveys.Any(y => y.SurveyInstanceID == 1)              
            );
        }
       
    }
}



