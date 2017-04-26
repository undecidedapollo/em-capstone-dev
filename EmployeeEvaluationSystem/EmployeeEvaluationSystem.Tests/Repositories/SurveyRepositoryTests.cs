using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Tests.Repositories
{

    [TestClass]
    public class SurveyRepositoryTests
    {

        [TestMethod]
        public void GetAvailableSurveyWithOneReturnsOne()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.SurveysAvailables, new List<SurveysAvailable>
                {
                    new SurveysAvailable{ID = 10, CohortID = 5}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var survey = unitOfWork.Surveys.GetAnAvailableSurveyForCohort(null, 10);

                Assert.IsNotNull(survey);
                Assert.AreEqual(survey.ID, 10);
            }
        }

        [TestMethod]
        public void GetAvailableSurveyWithNoneReturnsNone()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create();

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var survey = unitOfWork.Surveys.GetAnAvailableSurveyForCohort(null, 10);


                Assert.IsNull(survey);
            }
        }


        [TestMethod]
        public void GetPendingSurveyWithOneReturnsOne()
        {
            var theGuid = Guid.NewGuid();


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey{Id = theGuid, IsDeleted = false}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var survey = unitOfWork.Surveys.GetPendingSurveySYSTEM(theGuid);


                Assert.IsNotNull(survey);
                Assert.AreEqual(survey.Id, theGuid);
            }
        }

        [TestMethod]
        public void GetPendingSurveyWithOneDeletedReturnsNone()
        {
            var theGuid = Guid.NewGuid();


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey{Id = theGuid, IsDeleted = true}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var survey = unitOfWork.Surveys.GetPendingSurveySYSTEM(theGuid);


                Assert.IsNull(survey);
            }
        }

        [TestMethod]
        public void GetPendingSurveyWithNoneReturnsNone()
        {
            var theGuid = Guid.NewGuid();

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create();

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var survey = unitOfWork.Surveys.GetPendingSurveySYSTEM(theGuid);


                Assert.IsNull(survey);
            }
        }

        [TestMethod]
        public void CanExistingUserTakeSurveyShouldBeTrue()
        {
            var theGuid = Guid.NewGuid();


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey{Id = theGuid, IsDeleted = false, UserTakenById = "123"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var result = unitOfWork.Surveys.CanExistingUserTakeSurvey("123", theGuid);


                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void CanExistingUserTakeSurveyIsDeletedShouldBeFalse()
        {
            var theGuid = Guid.NewGuid();


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey{Id = theGuid, IsDeleted = true, UserTakenById = "123"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var result = unitOfWork.Surveys.CanExistingUserTakeSurvey("123", theGuid);


                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void CanExistingUserTakeSurveyWrongIdShouldBeFalse()
        {
            var theGuid = Guid.NewGuid();


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey{Id = theGuid, IsDeleted = true, UserTakenById = "123"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var result = unitOfWork.Surveys.CanExistingUserTakeSurvey("456", theGuid);


                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void CanGuestUserTakeSurveyShouldBeTrue()
        {
            var theGuid = Guid.NewGuid();


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey{Id = theGuid, IsDeleted = false, Email = "123"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var result = unitOfWork.Surveys.CanGuestUserTakeSurvey("123", theGuid);


                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void CanGuestUserTakeSurveyIsDeletedShouldBeFalse()
        {
            var theGuid = Guid.NewGuid();


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey{Id = theGuid, IsDeleted = true, Email = "123"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var result = unitOfWork.Surveys.CanGuestUserTakeSurvey("123", theGuid);


                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void CanGuestUserTakeSurveyWrongIdShouldBeFalse()
        {
            var theGuid = Guid.NewGuid();


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey{Id = theGuid, IsDeleted = true, Email = "123"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var result = unitOfWork.Surveys.CanGuestUserTakeSurvey("456", theGuid);


                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void GetUserSurveyRoleOneReturnsOne()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.UserSurveyRoles, new List<UserSurveyRole>
                {
                    new UserSurveyRole{ ID = 1, IsDeleted = false, Name = "test"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var result = unitOfWork.Surveys.GetUserSurveyRole(1);


                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.ID);
                Assert.IsFalse( result.IsDeleted);
            }
        }

        [TestMethod]
        public void GetUserSurveyRoleIsDeletedReturnsNone()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.UserSurveyRoles, new List<UserSurveyRole>
                {
                    new UserSurveyRole{ ID = 1, IsDeleted = true, Name = "test"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var result = unitOfWork.Surveys.GetUserSurveyRole(1);


                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void GetUserSurveyRoleWrongIdReturnsNone()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.UserSurveyRoles, new List<UserSurveyRole>
                {
                    new UserSurveyRole{ ID = 1, IsDeleted = false, Name = "test"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var result = unitOfWork.Surveys.GetUserSurveyRole(2);


                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void CreatePendingSurveyForExistingUserMakesOne()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.UserSurveyRoles, new List<UserSurveyRole>
                {
                    new UserSurveyRole{ ID = 1, IsDeleted = false, Name = "test"}
                })
                .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser{Id = "123" }
                })
                .InitializeOne(y => y.SurveysAvailables, new List<SurveysAvailable>
                {
                    new SurveysAvailable{ID = 10, CohortID = 5}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var result = unitOfWork.Surveys.CreatePendingSurveyForExistingUser("123", 1, 10);


                Assert.IsNotNull(result);
                Assert.AreEqual(result.UserSurveyForId, "123");
                Assert.AreEqual(result.UserTakenById, "123");
                Assert.IsFalse(result.IsDeleted);
            }
        }

        [TestMethod]
        public void CreatePendingSurveyForExistingUserNoRoleFails()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser{Id = "123" }
                })
                .InitializeOne(y => y.SurveysAvailables, new List<SurveysAvailable>
                {
                    new SurveysAvailable{ID = 10, CohortID = 5}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {

                try
                {
                    var result = unitOfWork.Surveys.CreatePendingSurveyForExistingUser("123", 1, 10);
                    Assert.Fail();
                }
                catch
                {

                }
            }
        }

        [TestMethod]
        public void CreatePendingSurveyForExistingUserNoUserFails()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.UserSurveyRoles, new List<UserSurveyRole>
                {
                    new UserSurveyRole{ ID = 1, IsDeleted = false, Name = "test"}
                })
                .InitializeOne(y => y.SurveysAvailables, new List<SurveysAvailable>
                {
                    new SurveysAvailable{ID = 10, CohortID = 5}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {

                try
                {
                    var result = unitOfWork.Surveys.CreatePendingSurveyForExistingUser("123", 1, 10);
                    Assert.Fail();
                }
                catch
                {

                }
            }
        }

        [TestMethod]
        public void CreatePendingSurveyForExistingUserNoSurveyAvailableFails()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.UserSurveyRoles, new List<UserSurveyRole>
                {
                    new UserSurveyRole{ ID = 1, IsDeleted = false, Name = "test"}
                })
                                .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser{Id = "123" }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {

                try
                {
                    var result = unitOfWork.Surveys.CreatePendingSurveyForExistingUser("123", 1, 10);
                    Assert.Fail();
                }
                catch
                {

                }
            }
        }

        [TestMethod]
        public void GetAllSurveysTwoReturnsTwo()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.Surveys, new List<Survey>
                {
                    new Survey{ID = 10},
                    new Survey{ID = 20}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var survey = unitOfWork.Surveys.GetAllSurveys(null);


                Assert.IsNotNull(survey);
                Assert.IsTrue(survey.Any(y => y.ID == 10));
                Assert.IsTrue(survey.Any(y => y.ID == 20));
            }
        }

        [TestMethod]
        public void GetAllSurveysNoneReturnsNone()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create();


            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var survey = unitOfWork.Surveys.GetAllSurveys(null);


                Assert.IsNotNull(survey);
                Assert.IsFalse(survey.Any());
            }
        }

        [TestMethod]
        public void GetUserSurveysOneReturnsOne()
        {
            var theGuid1 = Guid.NewGuid();
            var theGuid2 = Guid.NewGuid();


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey{Id = theGuid1, UserTakenById = "123"},
                    new PendingSurvey{Id = theGuid2, UserTakenById = "456"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var survey = unitOfWork.Surveys.GetAllSurveysForUser("123");


                Assert.IsNotNull(survey);
                Assert.IsTrue(survey.Any(y => y.Id == theGuid1 && y.UserTakenById == "123"));
                Assert.IsFalse(survey.Any(y => y.Id == theGuid2 || y.UserTakenById == "456"));
            }
        }

        [TestMethod]
        public void GetUserSurveysNoneReturnsNone()
        {
            var theGuid1 = Guid.NewGuid();
            var theGuid2 = Guid.NewGuid();


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>
                {
                    new PendingSurvey{Id = theGuid1, UserTakenById = "456"},
                    new PendingSurvey{Id = theGuid2, UserTakenById = "456"}
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var survey = unitOfWork.Surveys.GetAllSurveysForUser("123");


                Assert.IsNotNull(survey);
                Assert.IsFalse(survey.Any());
            }
        }




    }
}
