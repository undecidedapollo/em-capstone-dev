using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using System.Web.Mvc;
using EmployeeEvaluationSystem.MVC.Controllers;
using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using System.Collections.Generic;
using System.Linq;

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
    }
}
