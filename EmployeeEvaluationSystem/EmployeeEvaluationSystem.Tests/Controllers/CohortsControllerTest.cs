using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using System.Web.Mvc;
using EmployeeEvaluationSystem.MVC.Controllers;
using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using EmployeeEvaluationSystem.MVC.Models;
using EmployeeEvaluationSystem.MVC.Models.Survey;

namespace EmployeeEvaluationSystem.Tests.Controllers
{
    [TestClass]
    public class CohortsControllerTest
    {
        [TestMethod]
        public void IndexNoIdReturnsCohortList()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.Cohorts, new List<Cohort>
            {
                            new Cohort{ID = 1},
                            new Cohort {ID = 2}

            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Index(null);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<CohortIndexViewModel>(routeResult.Model);
            Assert.IsNotNull(modelResult?.Cohorts);

            UTH.IsTrue(
                modelResult.Cohorts.Any(y => y.ID == 1),
                modelResult.Cohorts.Any(y => y.ID == 2),
                modelResult.Cohorts.Count() == 2
                );
        }

        [TestMethod]
        public void DetailsIdReturnsCohort()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.Cohorts, new List<Cohort>
            {
                            new Cohort{ID = 1}
            }).InitializeOne(y => y.Surveys, new List<Survey>
            {
                new Survey
                {
                    ID = 1,
                    IsDeleted = false,
                    Name = "Test1"

                },
                new Survey
                {
                    ID = 2,
                    IsDeleted = false,
                    Name = "Test2"
                }
            }).InitializeOne(y => y.SurveysAvailables, new List<SurveysAvailable>
            {
                new SurveysAvailable
                {
                    ID = 1,
                    CohortID = 1,
                    SurveyID = 1,
                    SurveyTypeId = 1,
                    DateOpen = DateTime.Now,
                    DateClosed = DateTime.Now.AddHours(1),
                    IsDeleted = false,
                    Survey = new Survey
                    {
                        Name = "123"
                    },
                    SurveyType = new SurveyType
                    {
                        Name = "123"
                    }
                },
                new SurveysAvailable
                {
                    ID = 2,
                    CohortID = 1,
                    SurveyID = 2,
                    SurveyTypeId = 2,
                    DateOpen = DateTime.Now,
                    DateClosed = DateTime.Now.AddHours(1),
                    IsDeleted = false,
                    Survey = new Survey
                    {
                        Name = "123"
                    },
                    SurveyType = new SurveyType
                    {
                        Name = "123"
                    }
                }
            }).InitializeOne(y => y.SurveyTypes, new List<SurveyType>
            {
                new SurveyType
                {
                    ID = 1,
                    Name = "Type1"
                },
                new SurveyType
                {
                    ID = 2,
                    Name = "Type2"
                }
            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Details(1);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<CohortDetailsViewmodel>(routeResult.Model);
            Assert.IsNotNull(modelResult?.Surveys);
            Assert.IsNotNull(modelResult?.TheCohort);

            UTH.IsTrue(
                modelResult.Surveys.Any(y => y.Id == 1),
                modelResult.Surveys.Any(y => y.Id == 2),
                modelResult.Surveys.Count() == 2
                );
        }

        [TestMethod]
        public void CreateCohortGetActionTest()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser
                {
                    Id = "1"
                }
            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Create();

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<CreateCohortViewModel>(routeResult.Model);
            Assert.IsNotNull(modelResult?.Users);
            Assert.IsNotNull(modelResult?.Cohort);

            UTH.IsTrue(
                modelResult.Users.Any(y => y.Id.Equals("1")),
                modelResult.Users.Count() == 1
                );
        }

        [TestMethod]
        public void CreateCohortPostActionTest()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser
                {
                    Id = "1"
                }
            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Create();

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<CreateCohortViewModel>(routeResult.Model);
            Assert.IsNotNull(modelResult?.Users);
            Assert.IsNotNull(modelResult?.Cohort);

            UTH.IsTrue(
                modelResult.Users.Any(y => y.Id.Equals("1")),
                modelResult.Users.Count() == 1
                );
        }
    }
}
