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
    }
}
