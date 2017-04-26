using System;
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
        public void GetCohortWith1Returns1()
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
            }
        }


        [TestMethod]
        public void DeleteCohortTest()
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
    }
}

