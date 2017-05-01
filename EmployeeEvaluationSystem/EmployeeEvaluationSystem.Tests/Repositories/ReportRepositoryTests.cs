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
    public class ReportRepositoryTests
    {
        #region One User Tests

        [TestMethod]
        public void ReportGenerationForUserNoUserHasProperData()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            var c1 = new Category
            {
                ID = 1
            };

            var c2 = new Category
            {
                ID = 2
            };

            var q1 = new Question
            {
                ID = 1,
                Category = c1,
                CategoryID = c1.ID
            };

            var q2 = new Question
            {
                ID = 2,
                Category = c1,
                CategoryID = c1.ID
            };

            var q3 = new Question
            {
                ID = 3,
                Category = c2,
                CategoryID = c2.ID
            };


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>()
                {


                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var report = unitOfWork.Reports.GetDetailsForReport("123", 1);


                Assert.IsNotNull(report);

                UTH.IsTrue(
                    !report.Any(z => z.Id == 1),
                    report.Any(z => z.Id == null),
                    report.FirstOrDefault(z => z.Id == null).Questions.Count == 0
                    );
            }
        }

        [TestMethod]
        public void ReportGenerationForUserNoRaterHasProperData()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            var c1 = new Category
            {
                ID = 1
            };

            var c2 = new Category
            {
                ID = 2
            };

            var q1 = new Question
            {
                ID = 1,
                Category = c1,
                CategoryID = c1.ID
            };

            var q2 = new Question
            {
                ID = 2,
                Category = c1,
                CategoryID = c1.ID
            };

            var q3 = new Question
            {
                ID = 3,
                Category = c2,
                CategoryID = c2.ID
            };


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>()
                {
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 1,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 1,
                            Name ="Self"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 1
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 2
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 3
                                }
                            }
                        }
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var report = unitOfWork.Reports.GetDetailsForReport("123", 1);


                Assert.IsNotNull(report);

                UTH.IsTrue(
                    report.Any(z => z.Id == 1),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 1),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 3),
                    report.Any(z => z.Id == null),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 1),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 3)
                    );
            }
        }


        [TestMethod]
        public void ReportGenerationForUserOneRaterHasProperData()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            var c1 = new Category
            {
                ID = 1
            };

            var c2 = new Category
            {
                ID = 2
            };

            var q1 = new Question
            {
                ID = 1,
                Category = c1,
                CategoryID = c1.ID
            };

            var q2 = new Question
            {
                ID = 2,
                Category = c1,
                CategoryID = c1.ID
            };

            var q3 = new Question
            {
                ID = 3,
                Category = c2,
                CategoryID = c2.ID
            };


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>()
                {
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 1,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 1,
                            Name ="Self"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 1
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 2
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 3
                                }
                            }
                        }
                    },
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 2,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 2,
                            Name ="Coworker"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 3
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 4
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 7
                                }
                            }
                        }
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var report = unitOfWork.Reports.GetDetailsForReport("123", 1);


                Assert.IsNotNull(report);

                UTH.IsTrue(
                    report.Any(z => z.Id == 1),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 1),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 3),
                    report.Any(z => z.Id == 2),
                    report.FirstOrDefault(z => z.Id == 2).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == 2).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 4),
                    report.FirstOrDefault(z => z.Id == 2).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 7),
                    report.Any(z => z.Id == null),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 5)
                    );
            }
        }

        [TestMethod]
        public void ReportGenerationForUserThreeRaterHasProperData()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            var c1 = new Category
            {
                ID = 1
            };

            var c2 = new Category
            {
                ID = 2
            };

            var q1 = new Question
            {
                ID = 1,
                Category = c1,
                CategoryID = c1.ID
            };

            var q2 = new Question
            {
                ID = 2,
                Category = c1,
                CategoryID = c1.ID
            };

            var q3 = new Question
            {
                ID = 3,
                Category = c2,
                CategoryID = c2.ID
            };


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>()
                {
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 1,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 1,
                            Name ="Self"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 1
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 2
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 3
                                }
                            }
                        }
                    },
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 2,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 2,
                            Name ="Coworker"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 3
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 4
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 7
                                }
                            }
                        }
                    },
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 2,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 2,
                            Name ="Coworker"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 1
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 2
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 3
                                }
                            }
                        }
                    }
                    ,
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 3,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 3,
                            Name ="Supervisor"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 3
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 4
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 7
                                }
                            }
                        }
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var report = unitOfWork.Reports.GetDetailsForReport("123", 1);


                Assert.IsNotNull(report);

                UTH.IsTrue(
                    report.Any(z => z.Id == 1),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 1),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 3),
                    report.Any(z => z.Id == 2),
                    report.FirstOrDefault(z => z.Id == 2).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == 2).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == 2).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 5),
                    report.Any(z => z.Id == 3),
                    report.FirstOrDefault(z => z.Id == 3).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == 3).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 4),
                    report.FirstOrDefault(z => z.Id == 3).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 7),
                    report.Any(z => z.Id == null),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 5)
                    );
            }
        }





        #endregion

        #region Cohort User Tests


        [TestMethod]
        public void ReportGenerationForCohortNoUserHasProperData()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            var c1 = new Category
            {
                ID = 1
            };

            var c2 = new Category
            {
                ID = 2
            };

            var q1 = new Question
            {
                ID = 1,
                Category = c1,
                CategoryID = c1.ID
            };

            var q2 = new Question
            {
                ID = 2,
                Category = c1,
                CategoryID = c1.ID
            };

            var q3 = new Question
            {
                ID = 3,
                Category = c2,
                CategoryID = c2.ID
            };


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>()
                {


                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var report = unitOfWork.Reports.GetDetailsForReport(1);


                Assert.IsNotNull(report);

                UTH.IsTrue(
                    !report.Any(z => z.Id == 1),
                    report.Any(z => z.Id == null),
                    report.FirstOrDefault(z => z.Id == null).Questions.Count == 0
                    );
            }
        }

        [TestMethod]
        public void ReportGenerationForCohortOneUserNoRaterHasProperData()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            var c1 = new Category
            {
                ID = 1
            };

            var c2 = new Category
            {
                ID = 2
            };

            var q1 = new Question
            {
                ID = 1,
                Category = c1,
                CategoryID = c1.ID
            };

            var q2 = new Question
            {
                ID = 2,
                Category = c1,
                CategoryID = c1.ID
            };

            var q3 = new Question
            {
                ID = 3,
                Category = c2,
                CategoryID = c2.ID
            };


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>()
                {
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 1,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 1,
                            Name ="Self"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 1
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 2
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 3
                                }
                            }
                        }
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var report = unitOfWork.Reports.GetDetailsForReport(1);


                Assert.IsNotNull(report);

                UTH.IsTrue(
                    report.Any(z => z.Id == 1),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 1),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 3),
                    report.Any(z => z.Id == null),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 1),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 3)
                    );
            }
        }


        [TestMethod]
        public void ReportGenerationForCohortTwoUserNoRaterHasProperData()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            var c1 = new Category
            {
                ID = 1
            };

            var c2 = new Category
            {
                ID = 2
            };

            var q1 = new Question
            {
                ID = 1,
                Category = c1,
                CategoryID = c1.ID
            };

            var q2 = new Question
            {
                ID = 2,
                Category = c1,
                CategoryID = c1.ID
            };

            var q3 = new Question
            {
                ID = 3,
                Category = c2,
                CategoryID = c2.ID
            };


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>()
                {
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 1,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 1,
                            Name ="Self"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 1
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 2
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 3
                                }
                            }
                        }
                    },
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "456",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 1,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 1,
                            Name ="Self"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 3
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 4
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 7
                                }
                            }
                        }
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var report = unitOfWork.Reports.GetDetailsForReport(1);


                Assert.IsNotNull(report);

                UTH.IsTrue(
                    report.Any(z => z.Id == 1),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 5),
                    report.Any(z => z.Id == null),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 5)
                    );
            }
        }

        [TestMethod]
        public void ReportGenerationForCohortTwoUserSixRaterHasProperData()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            var c1 = new Category
            {
                ID = 1
            };

            var c2 = new Category
            {
                ID = 2
            };

            var q1 = new Question
            {
                ID = 1,
                Category = c1,
                CategoryID = c1.ID
            };

            var q2 = new Question
            {
                ID = 2,
                Category = c1,
                CategoryID = c1.ID
            };

            var q3 = new Question
            {
                ID = 3,
                Category = c2,
                CategoryID = c2.ID
            };


            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.PendingSurveys, new List<PendingSurvey>()
                {
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 1,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 1,
                            Name ="Self"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 1
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 2
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 3
                                }
                            }
                        }
                    },
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 2,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 2,
                            Name ="Coworker"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 3
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 4
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 7
                                }
                            }
                        }
                    },
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 2,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 2,
                            Name ="Coworker"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 1
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 2
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 3
                                }
                            }
                        }
                    }
                    ,
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "123",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 3,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 3,
                            Name ="Supervisor"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 3
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 4
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 7
                                }
                            }
                        }
                    },
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "456",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 1,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 1,
                            Name ="Self"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 3
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 4
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 7
                                }
                            }
                        }
                    },
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "456",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 2,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 2,
                            Name ="Coworker"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 3
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 4
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 7
                                }
                            }
                        }
                    },
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "456",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 2,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 2,
                            Name ="Coworker"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 1
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 2
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 3
                                }
                            }
                        }
                    }
                    ,
                    new PendingSurvey{
                        Id = guid1,
                        UserSurveyForId = "456",
                        SurveyAvailToMeID = 1,
                        UserSurveyRoleID = 3,
                        UserSurveyRole = new UserSurveyRole{
                            ID = 3,
                            Name ="Supervisor"
                        },
                        SurveyInstance = new SurveyInstance{
                            DateFinished = DateTime.UtcNow,
                            AnswerInstances = new List<AnswerInstance>
                            {
                                new AnswerInstance
                                {
                                    Question = q1,
                                    QuestionID = q1.ID,
                                    ResponseNum = 1
                                },
                                new AnswerInstance
                                {
                                    Question = q2,
                                    QuestionID = q2.ID,
                                    ResponseNum = 2
                                },
                                new AnswerInstance
                                {
                                    Question = q3,
                                    QuestionID = q3.ID,
                                    ResponseNum = 3
                                }
                            }
                        }
                    }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var report = unitOfWork.Reports.GetDetailsForReport(1);


                Assert.IsNotNull(report);

                UTH.IsTrue(
                    report.Any(z => z.Id == 1),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == 1).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 5),
                    report.Any(z => z.Id == 2),
                    report.FirstOrDefault(z => z.Id == 2).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == 2).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == 2).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 5),
                    report.Any(z => z.Id == 3),
                    report.FirstOrDefault(z => z.Id == 3).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == 3).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == 3).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 5),
                    report.Any(z => z.Id == null),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 1 && z.RatingValue == 2),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 2 && z.RatingValue == 3),
                    report.FirstOrDefault(z => z.Id == null).Questions.Any(z => z.QuestionId == 3 && z.RatingValue == 5)
                    );
            }
        }


        #endregion


    }
}
