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
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories;
using Moq;
using EmployeeEvaluationSystem.MVC.Models.Survey;

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



        [TestMethod]
        public void ContinueSurveyReturnsView()
        {

            var ctrl = new SurveyController(null, null, null);

            var result = ctrl.ContinueSurvey();

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);

        }

        [TestMethod]
        public void SurveyMissingDataReturnsView()
        {

            var ctrl = new SurveyController(null, null, null);

            var result = ctrl.SurveyMissingData();

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);

        }

        #region GetSurveyPage


        [TestMethod]
        public void GetSurveyPageExistingReturnsView()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123", SurveyInstance = new SurveyInstance{ ID = 5, DateStarted = DateTime.UtcNow, SurveyID = 1}, SurveysAvailable = new SurveysAvailable{ ID = 5, SurveyID = 1, DateOpen = DateTime.UtcNow - TimeSpan.FromDays(10), DateClosed = DateTime.UtcNow + TimeSpan.FromDays(10) } }

            })
            .InitializeOne(y => y.Categories, new List<Category>
            {
                            new Category { SurveyID = 1, ID = 10}

            });

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var newMoq = new Mock<SurveyRepository>(newUOW.Object, ct);

            newMoq.Setup(y => y.LockAndGetSurvey(It.IsAny<Guid>(), null)).Returns(new LockAndGetSurvey_Result { StatusGuid = guid });
            newMoq.Setup(y => y.CanGuestUserTakeSurvey(It.IsAny<string>(), It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetFirstCategory(It.IsAny<int>())).CallBase();
            newMoq.Setup(y => y.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tuple<Question, AnswerInstance>>());
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            var newObj = newMoq.Object;

            newUOW.SetupGet(y => y.Surveys).Returns(newObj);


            var ctrl = new SurveyController(null, null, newUOW.Object.GetCreator());

            var result = ctrl.SurveyPage(guid, "123");

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<SurveyPageViewModel>(routeResult.Model);


            UTH.IsTrue(
                modelResult.Category.Id == 10,
                modelResult.PendingSurveyId == guid,
                modelResult.Questions.Count == 0,
                modelResult.SurveyInstanceId == 5
                );
        }

        [TestMethod]
        public void GetSurveyPageNewReturnsView()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123", SurveysAvailable = new SurveysAvailable{ ID = 5, SurveyID = 1, DateOpen = DateTime.UtcNow - TimeSpan.FromDays(10), DateClosed = DateTime.UtcNow + TimeSpan.FromDays(10) } }

            })
            .InitializeOne(y => y.Categories, new List<Category>
            {
                            new Category { SurveyID = 1, ID = 10}

            });

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var newMoq = new Mock<SurveyRepository>(newUOW.Object, ct);

            newMoq.Setup(y => y.LockAndGetSurvey(It.IsAny<Guid>(), null)).Returns(new LockAndGetSurvey_Result { StatusGuid = guid });
            newMoq.Setup(y => y.CanGuestUserTakeSurvey(It.IsAny<string>(), It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetFirstCategory(It.IsAny<int>())).CallBase();
            newMoq.Setup(y => y.CreateSurveyInstanceForGuestUser(It.IsAny<string>(), It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetSurveyInstanceByIdSYSTEM(It.IsAny<int>())).CallBase();
            newMoq.Setup(y => y.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tuple<Question, AnswerInstance>>());
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            var newObj = newMoq.Object;

            newUOW.SetupGet(y => y.Surveys).Returns(newObj);


            var ctrl = new SurveyController(null, null, newUOW.Object.GetCreator());

            var result = ctrl.SurveyPage(guid, "123");

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<SurveyPageViewModel>(routeResult.Model);


            UTH.IsTrue(
                modelResult.Category.Id == 10,
                modelResult.PendingSurveyId == guid,
                modelResult.Questions.Count == 0
                );
        }

        [TestMethod]
        public void GetSurveyPageTooLateReturnsRedirect()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123", SurveyInstance = new SurveyInstance{ ID = 5, DateStarted = DateTime.UtcNow, SurveyID = 1}, SurveysAvailable = new SurveysAvailable{ ID = 5, SurveyID = 1, DateOpen = DateTime.UtcNow - TimeSpan.FromDays(10), DateClosed = DateTime.UtcNow - TimeSpan.FromDays(9) } }

            })
            .InitializeOne(y => y.Categories, new List<Category>
            {
                            new Category { SurveyID = 1, ID = 10}

            });

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var newMoq = new Mock<SurveyRepository>(newUOW.Object, ct);

            newMoq.Setup(y => y.LockAndGetSurvey(It.IsAny<Guid>(), null)).Returns(new LockAndGetSurvey_Result { StatusGuid = guid });
            newMoq.Setup(y => y.CanGuestUserTakeSurvey(It.IsAny<string>(), It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetFirstCategory(It.IsAny<int>())).CallBase();
            newMoq.Setup(y => y.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tuple<Question, AnswerInstance>>());
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            var newObj = newMoq.Object;

            newUOW.SetupGet(y => y.Surveys).Returns(newObj);


            var ctrl = new SurveyController(null, null, newUOW.Object.GetCreator());

            var result = ctrl.SurveyPage(guid, "123");

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);
            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "SurveyTooLate")
                );

        }

        [TestMethod]
        public void GetSurveyPageTooSoonReturnsRedirect()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123", SurveyInstance = new SurveyInstance{ ID = 5, DateStarted = DateTime.UtcNow, SurveyID = 1}, SurveysAvailable = new SurveysAvailable{ ID = 5, SurveyID = 1, DateOpen = DateTime.UtcNow + TimeSpan.FromDays(1), DateClosed = DateTime.UtcNow + TimeSpan.FromDays(9) } }

            })
            .InitializeOne(y => y.Categories, new List<Category>
            {
                            new Category { SurveyID = 1, ID = 10}

            });

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var newMoq = new Mock<SurveyRepository>(newUOW.Object, ct);

            newMoq.Setup(y => y.LockAndGetSurvey(It.IsAny<Guid>(), null)).Returns(new LockAndGetSurvey_Result { StatusGuid = guid });
            newMoq.Setup(y => y.CanGuestUserTakeSurvey(It.IsAny<string>(), It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetFirstCategory(It.IsAny<int>())).CallBase();
            newMoq.Setup(y => y.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tuple<Question, AnswerInstance>>());
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            var newObj = newMoq.Object;

            newUOW.SetupGet(y => y.Surveys).Returns(newObj);


            var ctrl = new SurveyController(null, null, newUOW.Object.GetCreator());

            var result = ctrl.SurveyPage(guid, "123");

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);
            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "SurveyTooSoon")
                );

        }

        [TestMethod]
        public void GetSurveyNoSurveyAvailableReturnsRedirect()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123", SurveyInstance = new SurveyInstance{ ID = 5, DateStarted = DateTime.UtcNow, SurveyID = 1}}

            })
            .InitializeOne(y => y.Categories, new List<Category>
            {
                            new Category { SurveyID = 1, ID = 10}

            });

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var newMoq = new Mock<SurveyRepository>(newUOW.Object, ct);

            newMoq.Setup(y => y.LockAndGetSurvey(It.IsAny<Guid>(), null)).Returns(new LockAndGetSurvey_Result { StatusGuid = guid });
            newMoq.Setup(y => y.CanGuestUserTakeSurvey(It.IsAny<string>(), It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetFirstCategory(It.IsAny<int>())).CallBase();
            newMoq.Setup(y => y.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tuple<Question, AnswerInstance>>());
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            var newObj = newMoq.Object;

            newUOW.SetupGet(y => y.Surveys).Returns(newObj);


            var ctrl = new SurveyController(null, null, newUOW.Object.GetCreator());

            var result = ctrl.SurveyPage(guid, "123");

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);
            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "SurveyError")
                );

        }

        [TestMethod]
        public void GetSurveyNoPendingReturnsRedirect()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123", SurveyInstance = new SurveyInstance{ ID = 5, DateStarted = DateTime.UtcNow, SurveyID = 1}}

            })
            .InitializeOne(y => y.Categories, new List<Category>
            {
                            new Category { SurveyID = 1, ID = 10}

            });

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var newMoq = new Mock<SurveyRepository>(newUOW.Object, ct);

            newMoq.Setup(y => y.LockAndGetSurvey(It.IsAny<Guid>(), null)).Returns(new LockAndGetSurvey_Result { StatusGuid = guid });
            newMoq.Setup(y => y.CanGuestUserTakeSurvey(It.IsAny<string>(), It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetFirstCategory(It.IsAny<int>())).CallBase();
            newMoq.Setup(y => y.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tuple<Question, AnswerInstance>>());
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            var newObj = newMoq.Object;

            newUOW.SetupGet(y => y.Surveys).Returns(newObj);


            var ctrl = new SurveyController(null, null, newUOW.Object.GetCreator());

            var result = ctrl.SurveyPage(guid, "456");

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);
            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "SurveyNoPermission")
                );

        }

        [TestMethod]
        public void GetSurveyLockedSurveyReturnsRedirect()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123", SurveyInstance = new SurveyInstance{ ID = 5, DateStarted = DateTime.UtcNow, SurveyID = 1}}

            })
            .InitializeOne(y => y.Categories, new List<Category>
            {
                            new Category { SurveyID = 1, ID = 10}

            });

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var newMoq = new Mock<SurveyRepository>(newUOW.Object, ct);

            newMoq.Setup(y => y.LockAndGetSurvey(It.IsAny<Guid>(), null)).Returns(null as LockAndGetSurvey_Result);
            newMoq.Setup(y => y.CanGuestUserTakeSurvey(It.IsAny<string>(), It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetFirstCategory(It.IsAny<int>())).CallBase();
            newMoq.Setup(y => y.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tuple<Question, AnswerInstance>>());
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            var newObj = newMoq.Object;

            newUOW.SetupGet(y => y.Surveys).Returns(newObj);


            var ctrl = new SurveyController(null, null, newUOW.Object.GetCreator());

            var result = ctrl.SurveyPage(guid, "123");

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);
            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "SurveyLocked")
                );

        }

        [TestMethod]
        public void GetSurveyNoLockGuidReturnsRedirect()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123", SurveyInstance = new SurveyInstance{ ID = 5, DateStarted = DateTime.UtcNow, SurveyID = 1}}

            })
            .InitializeOne(y => y.Categories, new List<Category>
            {
                            new Category { SurveyID = 1, ID = 10}

            });

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var newMoq = new Mock<SurveyRepository>(newUOW.Object, ct);

            newMoq.Setup(y => y.LockAndGetSurvey(It.IsAny<Guid>(), null)).Returns(new LockAndGetSurvey_Result { });
            newMoq.Setup(y => y.CanGuestUserTakeSurvey(It.IsAny<string>(), It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetFirstCategory(It.IsAny<int>())).CallBase();
            newMoq.Setup(y => y.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tuple<Question, AnswerInstance>>());
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            var newObj = newMoq.Object;

            newUOW.SetupGet(y => y.Surveys).Returns(newObj);


            var ctrl = new SurveyController(null, null, newUOW.Object.GetCreator());

            var result = ctrl.SurveyPage(guid, "123");

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);
            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "SurveyError")
                );

        }


        [TestMethod]
        public void GetSurveyPageFinishedReturnsRedirect()
        {
            var guid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
            {
                            new PendingSurvey{Id = guid,  Email = "123", SurveyInstance = new SurveyInstance{ ID = 5, DateStarted = DateTime.UtcNow, DateFinished = DateTime.UtcNow, SurveyID = 1}, SurveysAvailable = new SurveysAvailable{ ID = 5, SurveyID = 1, DateOpen = DateTime.UtcNow - TimeSpan.FromDays(10), DateClosed = DateTime.UtcNow + TimeSpan.FromDays(10) } }

            })
            .InitializeOne(y => y.Categories, new List<Category>
            {
                            new Category { SurveyID = 1, ID = 10}

            });

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var newMoq = new Mock<SurveyRepository>(newUOW.Object, ct);

            newMoq.Setup(y => y.LockAndGetSurvey(It.IsAny<Guid>(), null)).Returns(new LockAndGetSurvey_Result { StatusGuid = guid });
            newMoq.Setup(y => y.CanGuestUserTakeSurvey(It.IsAny<string>(), It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            newMoq.Setup(y => y.GetFirstCategory(It.IsAny<int>())).CallBase();
            newMoq.Setup(y => y.GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tuple<Question, AnswerInstance>>());
            newMoq.Setup(y => y.GetPendingSurveySYSTEM(It.IsAny<Guid>())).CallBase();
            var newObj = newMoq.Object;

            newUOW.SetupGet(y => y.Surveys).Returns(newObj);


            var ctrl = new SurveyController(null, null, newUOW.Object.GetCreator());

            var result = ctrl.SurveyPage(guid, "123");




            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);
            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "SurveyDone")
                );
        }


        #endregion






    }
}
