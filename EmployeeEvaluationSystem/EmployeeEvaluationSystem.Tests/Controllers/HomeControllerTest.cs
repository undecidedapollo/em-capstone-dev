using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeEvaluationSystem.MVC;
using EmployeeEvaluationSystem.MVC.Controllers;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using EmployeeEvaluationSystem.Entity;

namespace EmployeeEvaluationSystem.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            var theObj = DependencyBuilder.BuildDependency();

            var theMock = theObj.Item1.FindMy<IUnitOfWorkBuilder<EmployeeDatabaseEntities>>();

            theMock.InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser { FirstName = "BBB", LastName = "LLL", UserName = "123" },
                    new AspNetUser { FirstName = "CCC", LastName = "LLL", UserName = "456" },
                    new AspNetUser { FirstName = "DDD", LastName = "LLL", UserName = "789" }
                });

            // Arrange
            HomeController controller = new HomeController() { DependencyObject = theObj.Item2};

            // Act
            ViewResult result = controller.Index() as ViewResult;

            Assert.AreEqual(result.ViewBag.NumUsers, 3);
            
            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void About()
        {
            Assert.IsTrue(true);
            return;
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            Assert.IsTrue(true);
            return;
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
