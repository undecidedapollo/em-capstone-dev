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
        public void DetailsIdNotFoundReturnsNotFoundResult()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create();

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Details(1);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<HttpNotFoundResult>(result);


            UTH.IsTrue(
                routeResult.StatusCode == 404
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
        public void CreateCohortPostValidReturnsRedirectAndAdded()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser
                {
                    Id = "1"
                },
                 new AspNetUser
                {
                    Id = "2"
                },
                  new AspNetUser
                {
                    Id = "3"
                }
            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Create(new CreateCohortViewModel {
            Cohort = new PersonalCohortViewModel
            {
                Name = "stevo",
                Description = "plivo"
            }
            }, new List<string> { "1","2","3"});

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);

            var context = x.GetContext();


            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "SendEmailConfirmationTokenAsync"),
                context.Cohorts.Any(y => y.Name == "stevo" && y.CohortUsers.Count() == 3)
                );
        }

        [TestMethod]
        public void CreateCohortPostMissingUserThrowsException()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser
                {
                    Id = "1"
                },
                 new AspNetUser
                {
                    Id = "2"
                }
            });

            var ctrl = new CohortsController(x.GetCreator());

            try
            {
                var result = ctrl.Create(new CreateCohortViewModel
                {
                    Cohort = new PersonalCohortViewModel
                    {
                        Name = "stevo",
                        Description = "plivo"
                    }
                }, new List<string> { "1", "2", "3" });

                Assert.Fail();
            }
            catch (Exception)
            {

            }
           

            
        }

        [TestMethod]
        public void CreateCohortPostNoNameReturnsView()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser
                {
                    Id = "1"
                },
                 new AspNetUser
                {
                    Id = "2"
                },
                  new AspNetUser
                {
                    Id = "3"
                }
            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Create(new CreateCohortViewModel
            {
                Cohort = new PersonalCohortViewModel
                {
                   
                    Description = "plivo"
                }
            }, new List<string> { "1", "2", "3" });

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<CreateCohortViewModel>(routeResult.Model);
        }

        [TestMethod]
        public void CreateCohortPostNoDescriptionReturnsView()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser
                {
                    Id = "1"
                },
                 new AspNetUser
                {
                    Id = "2"
                },
                  new AspNetUser
                {
                    Id = "3"
                }
            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Create(new CreateCohortViewModel
            {
                Cohort = new PersonalCohortViewModel
                {
                    Name = "test"
                }
            }, new List<string> { "1", "2", "3" });

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<CreateCohortViewModel>(routeResult.Model);
        }

        [TestMethod]
        public void CreateCohortPostNullUsersReturnsView()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser
                {
                    Id = "1"
                },
                 new AspNetUser
                {
                    Id = "2"
                },
                  new AspNetUser
                {
                    Id = "3"
                }
            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Create(new CreateCohortViewModel
            {
                Cohort = new PersonalCohortViewModel
                {
                    Name = "test",
                    Description = "aaerea"
                }
            }, null);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<CreateCohortViewModel>(routeResult.Model);
        }


        [TestMethod]
        public void CreateCohortPostNoUsersReturnsView()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser
                {
                    Id = "1"
                },
                 new AspNetUser
                {
                    Id = "2"
                },
                  new AspNetUser
                {
                    Id = "3"
                }
            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Create(new CreateCohortViewModel
            {
                Cohort = new PersonalCohortViewModel
                {
                    Name = "test",
                    Description = "eardaslkj"
                }
            }, new List<string> { });

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<CreateCohortViewModel>(routeResult.Model);
        }


        [TestMethod]
        public void GETEditCohortReturnsCohort()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.Cohorts, new List<Cohort>
            {
                            new Cohort{ID = 1}
            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Edit(1);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(result);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<Cohort>(routeResult.Model);

            UTH.IsTrue(
                modelResult.ID == 1
                );
        }

        [TestMethod]
        public void GETEditCohortNotFoundReturnsNotFound()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.Cohorts, new List<Cohort>
            {
                            new Cohort{ID = 1}
            });

            var ctrl = new CohortsController(x.GetCreator());

            var result = ctrl.Edit(2);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<HttpNotFoundResult>(result);
            UTH.IsTrue(
                routeResult.StatusCode == 404
                );
        }

        [TestMethod]
        public void POSTEditCohortReturnsRedirect()
        {

            var coho = new Cohort { ID = 1, Name = "steve", Description = "jobs" };

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.Cohorts, new List<Cohort>
            {
                           coho
            });

            var ctrl = new CohortsController(x.GetCreator());

            var newCoho = new Cohort { ID = 1, Name = "tim", Description = "desc" };

            var result = ctrl.Edit(newCoho);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);
            

            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "Index"),
                coho.Name == "tim",
                coho.Description == "desc",
                coho != newCoho);
        }

        [TestMethod]
        public void POSTEditCohortNullReturnsNotFound()
        {

            var coho = new Cohort { ID = 1, Name = "steve", Description = "jobs" };

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.Cohorts, new List<Cohort>
            {
                           coho
            });

            var ctrl = new CohortsController(x.GetCreator());

            var newCoho = new Cohort { ID = 1, Name = "tim", Description = "desc" };

            var result = ctrl.Edit(null as Cohort);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<HttpStatusCodeResult>(result);


            UTH.IsTrue(
                routeResult.StatusCode == 400
                );
        }

        [TestMethod]
        public void POSTEditCohortNotFoundThrowsException()
        {

            var coho = new Cohort { ID = 1, Name = "steve", Description = "jobs" };

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.Cohorts, new List<Cohort>
            {
                           coho
            });

            var ctrl = new CohortsController(x.GetCreator());



            var newCoho = new Cohort { ID = 2, Name = "tim", Description = "desc" };
            try
            {
                var result = ctrl.Edit(newCoho);
                Assert.Fail();
            }
            catch (Exception)
            {

            }
        }

        [TestMethod]
        public void POSTEditCohortNullDescriptionLeavesOldDataAndReturnsRedirect()
        {

            var coho = new Cohort { ID = 1, Name = "steve", Description = "jobs" };

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.Cohorts, new List<Cohort>
            {
                           coho
            });

            var ctrl = new CohortsController(x.GetCreator());

            var newCoho = new Cohort { ID = 1, Name = "tim", };

            var result = ctrl.Edit(newCoho);

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(result);


            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "Index"),
                coho.Name == "tim",
                coho.Description == "jobs",
                coho != newCoho);
        }


    }
}
