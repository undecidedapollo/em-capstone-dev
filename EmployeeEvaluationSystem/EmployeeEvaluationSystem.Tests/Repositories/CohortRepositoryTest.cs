using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using System.Collections.Generic;

namespace EmployeeEvaluationSystem.Tests.Repositories
{
    [TestClass]
    public class CohortRepositoryTest
    {
        [TestMethod]
        public void GetCohortWithIDReturnsCohortWithGivenID()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create().InitializeOne(y => y.Cohorts, new List<Cohort>
                {
                    new Cohort
                    {
                        ID = 5,
                        IsDeleted = false,
                        Name = "Bradley"
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var cohort = unitOfWork.Cohorts.GetCohort(null, 5);


                Assert.IsNotNull(cohort);
                Assert.AreEqual(cohort.ID, 5);
            }
        }

        [TestMethod]
        public void GetCohortWithIDThatDoesNotExistReturnsNull()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create().InitializeOne(y => y.Cohorts, new List<Cohort>
                {
                    new Cohort
                    {
                        ID = 5,
                        IsDeleted = false,
                        Name = "Bradley"
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var cohort = unitOfWork.Cohorts.GetCohort(null, 6);

                Assert.IsNull(cohort);
            }
        }

        [TestMethod]
        public void DeleteCohortWithIDDeletesCorrectCohort()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create().InitializeOne(y => y.Cohorts, new List<Cohort>
                {
                    new Cohort
                    {
                        ID = 5,
                        IsDeleted = false,
                        Name="Bradley"
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var cohort = unitOfWork.Cohorts.GetCohort(null, 5);

                Assert.IsNotNull(cohort);
                Assert.AreEqual(cohort.ID, 5);

                unitOfWork.Cohorts.DeleteCohort(null, 5);

                cohort = unitOfWork.Cohorts.GetCohort(null, 5);

                Assert.IsNull(cohort);
            }
        }

        [TestMethod]
        public void DeleteCohortWithIDThatDoesNotExistThrowsException()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create().InitializeOne(y => y.Cohorts, new List<Cohort>
                {
                    new Cohort
                    {
                        ID = 5,
                        IsDeleted = false,
                        Name="Bradley"
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var cohort = unitOfWork.Cohorts.GetCohort(null, 5);

                Assert.IsNotNull(cohort);
                Assert.AreEqual(cohort.ID, 5);

                try
                {
                    var isDeleted = unitOfWork.Cohorts.DeleteCohort(null, 6);
                }
                catch
                {
                    
                }

            }
        }

        [TestMethod]
        public void EditCohortTest()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create().InitializeOne(y => y.Cohorts, new List<Cohort>
                {
                    new Cohort
                    {
                        ID = 5,
                        IsDeleted = false,
                        Name="Bad",
                        Description = "Bad"
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var cohort = new Cohort
                {
                    ID = 5,
                    IsDeleted = false,
                    Name = "Good",
                    Description = "Good"
                };

                cohort = unitOfWork.Cohorts.EditCohort(null, cohort);

                Assert.AreEqual(cohort.Name, "Good");
                Assert.AreEqual(cohort.Description, "Good");
            }
        }

        [TestMethod]
        public void AddCohortToDBTest()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create().InitializeOne(y => y.Cohorts, new List<Cohort>
                {
                    new Cohort
                    {
                        ID = 5,
                        IsDeleted = false,
                        Name="Bad",
                        Description = "Bad"
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var cohort = new Cohort
                {
                    ID = 6,
                    IsDeleted = false,
                    Name = "Good",
                    Description = "Good"
                };

                unitOfWork.Cohorts.AddCohortToDb(null, cohort);

                var cohorts = unitOfWork.Cohorts.GetAllCohorts(null).ToList();

                Assert.AreEqual(cohorts.Count, 2);
            }
        }

        [TestMethod]
        public void GetAllCohortsTest()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create().InitializeOne(y => y.Cohorts, new List<Cohort>
                {
                    new Cohort
                    {
                        ID = 5,
                        IsDeleted = false,
                        Name="Bad",
                        Description = "Bad"
                    },

                    new Cohort
                    {
                        ID = 6,
                        IsDeleted = false,
                        Name="Bad",
                        Description = "Bad"
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
               var cohorts = unitOfWork.Cohorts.GetAllCohorts(null).ToList();

                Assert.AreEqual(cohorts.Count, 2);
            }
        }

        [TestMethod]
        public void GetAllUsersNotPartOfACohortTest()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create().InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser
                    {
                        Id = "1",
                        FirstName = "Tod",
                        LastName = "Rod",
                        Email = "Tod.Rod@mailinator.com"
                    },

                    new AspNetUser
                    {
                        Id = "2",
                        FirstName = "Jay",
                        LastName = "Bay",
                        Email = "Jay.Bay@mailinator.com"
                    }
                }).InitializeOne(y => y.Cohorts, new List<Cohort>
                {
                    new Cohort
                    {
                        ID = 1,
                        Name = "TestCohort",
                        Description = "TestDescription",
                        IsDeleted = false
                    }
                }).InitializeOne(y => y.CohortUsers, new List<CohortUser>
                {
                    new CohortUser
                    {
                        UserID = "1",
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var cohort = unitOfWork.Cohorts.GetCohort(null, 1);

                var cohortUser = unitOfWork.CohortUsers.GetCohortUser(null, "1");

                var ASPNetUsers = unitOfWork.Users.GetAllUsers(null);

                cohortUser.AspNetUser = ASPNetUsers.First();

                cohortUser.Cohort = cohort;

                cohort.CohortUsers.Add(cohortUser);

                var usersNotInCohort = unitOfWork.Cohorts.GetAllUsersThatAreNotPartOfACohort(null);

                Assert.AreEqual(usersNotInCohort.Count(), 1);

                var user = usersNotInCohort.First();

                Assert.AreEqual(user.Id, "2");
            }
        }
    }
}

