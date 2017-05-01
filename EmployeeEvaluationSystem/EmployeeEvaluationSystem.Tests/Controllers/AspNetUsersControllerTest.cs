using System;
using EmployeeEvaluationSystem.MVC.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Tests.Controllers
{
    [TestClass]
    public class AspNetUsersControllerTest
    {
        [TestMethod]
        public void AspNetUserIndexTest()
        {
            AspNetUsersController aspUser = new AspNetUsersController();

            var view = aspUser.Index();
            UTH.CheckIsTypeNotNullAndGetObj<Task<ActionResult>>(view);
        }
    }
}
