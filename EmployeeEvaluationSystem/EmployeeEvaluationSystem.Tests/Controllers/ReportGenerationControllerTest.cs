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
using EmployeeEvaluationSystem.MVC.Models.Report;


namespace EmployeeEvaluationSystem.Tests.Controllers
{
    [TestClass]
    public class ReportGenerationControllerTest
    {       

        [TestMethod]
        public void ReportGenerationUserReturnsView()
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

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var survMoq = new Mock<SurveyRepository>(newUOW.Object, ct);
            var reportMoq = new Mock<ReportRepository>(newUOW.Object, ct);
            var userMoq = new Mock<UserRepository>(newUOW.Object, ct);




            reportMoq.Setup(y => y.GetDetailsForReport(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<ReportRole>() {
                new ReportRole
                {
                    Id = 1,
                    Name = "Self",
                    Questions = new List<ReportQuestionAverage>()
                    {
                        new ReportQuestionAverage
                        {
                            CategoryId = 1,
                            QuestionId = 1,
                            RatingValue = 1
                        },
                        new ReportQuestionAverage
                        {
                            CategoryId = 1,
                            QuestionId = 2,
                            RatingValue = 3
                        },
                        new ReportQuestionAverage
                        {
                            CategoryId = 2,
                            QuestionId = 3,
                            RatingValue = 52
                        }
                    }
                },
                new ReportRole
                {
                    Id = 2,
                    Name = "Gandolf",
                    Questions = new List<ReportQuestionAverage>()
                    {
                        new ReportQuestionAverage
                        {
                            CategoryId = 1,
                            QuestionId = 1,
                            RatingValue = 1
                        },
                        new ReportQuestionAverage
                        {
                            CategoryId = 1,
                            QuestionId = 2,
                            RatingValue = 3
                        },
                        new ReportQuestionAverage
                        {
                            CategoryId = 2,
                            QuestionId = 3,
                            RatingValue = 52
                        }
                    }
                }
            });


            survMoq.Setup(y => y.GetAnAvailableSurveyForCohortSYSTEM(It.IsAny<int>(), It.IsAny<bool>())).Returns(new SurveysAvailable
            {
                Survey = new Survey
                {
                    Name = "name"
                },
                SurveyType = new SurveyType
                {
                    Name = "type"
                }
            });

            userMoq.Setup(y => y.GetUser(It.IsAny<string>(), It.IsAny<string>())).Returns(new AspNetUser
            {
                FirstName = "Bradley",
                LastName = "Norman"
            });

            newUOW.SetupGet(y => y.Surveys).Returns(survMoq.Object);
            newUOW.SetupGet(y => y.Reports).Returns(reportMoq.Object);
            newUOW.SetupGet(y => y.Users).Returns(userMoq.Object);






            var ctrl = new ReportGenerationController(new PreExistingUnitOfWorkCreator { uow = newUOW.Object });
            var result = ctrl.ReportPage("123", 1);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<ReportDetailsViewModel>(routeResult.Model);


            UTH.IsTrue(
                !string.IsNullOrWhiteSpace(modelResult.Header),
                !string.IsNullOrWhiteSpace(modelResult.Header2),
                modelResult.ResponseItems.Count == 2,
                modelResult.ResponseItems.Any(y => y.Id == 1 && y.Questions.Count() == 3),
                modelResult.ResponseItems.Any(y => y.Id == 2 && y.Questions.Count() == 3)
                );



        }

        [TestMethod]
        public void ReportGenerationCohortReturnsView()
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

            var ct = x.GetContext();


            var newUOW = new Mock<UnitOfWork>(ct);


            var survMoq = new Mock<SurveyRepository>(newUOW.Object, ct);
            var reportMoq = new Mock<ReportRepository>(newUOW.Object, ct);
            var userMoq = new Mock<UserRepository>(newUOW.Object, ct);




            reportMoq.Setup(y => y.GetDetailsForReport(It.IsAny<int>())).Returns(new List<ReportRole>() {
                new ReportRole
                {
                    Id = 1,
                    Name = "Self",
                    Questions = new List<ReportQuestionAverage>()
                    {
                        new ReportQuestionAverage
                        {
                            CategoryId = 1,
                            QuestionId = 1,
                            RatingValue = 1
                        },
                        new ReportQuestionAverage
                        {
                            CategoryId = 1,
                            QuestionId = 2,
                            RatingValue = 3
                        },
                        new ReportQuestionAverage
                        {
                            CategoryId = 2,
                            QuestionId = 3,
                            RatingValue = 52
                        }
                    }
                },
                new ReportRole
                {
                    Id = 2,
                    Name = "Gandolf",
                    Questions = new List<ReportQuestionAverage>()
                    {
                        new ReportQuestionAverage
                        {
                            CategoryId = 1,
                            QuestionId = 1,
                            RatingValue = 1
                        },
                        new ReportQuestionAverage
                        {
                            CategoryId = 1,
                            QuestionId = 2,
                            RatingValue = 3
                        },
                        new ReportQuestionAverage
                        {
                            CategoryId = 2,
                            QuestionId = 3,
                            RatingValue = 52
                        }
                    }
                }
            });


            survMoq.Setup(y => y.GetAnAvailableSurveyForCohortSYSTEM(It.IsAny<int>(), It.IsAny<bool>())).Returns(new SurveysAvailable
            {
                Survey = new Survey
                {
                    Name = "name"
                },
                SurveyType = new SurveyType
                {
                    Name = "type"
                }
            });

            userMoq.Setup(y => y.GetUser(It.IsAny<string>(), It.IsAny<string>())).Returns(new AspNetUser
            {
                FirstName = "Bradley",
                LastName = "Norman"
            });

            newUOW.SetupGet(y => y.Surveys).Returns(survMoq.Object);
            newUOW.SetupGet(y => y.Reports).Returns(reportMoq.Object);
            newUOW.SetupGet(y => y.Users).Returns(userMoq.Object);






            var ctrl = new ReportGenerationController(new PreExistingUnitOfWorkCreator { uow = newUOW.Object });
            var result = ctrl.ReportPage(null, 1);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<ReportDetailsViewModel>(routeResult.Model);


            UTH.IsTrue(
                !string.IsNullOrWhiteSpace(modelResult.Header),
                !string.IsNullOrWhiteSpace(modelResult.Header2),
                modelResult.ResponseItems.Count == 2,
                modelResult.ResponseItems.Any(y => y.Id == 1 && y.Questions.Count() == 3),
                modelResult.ResponseItems.Any(y => y.Id == 2 && y.Questions.Count() == 3)
                );



        }
    }
}

