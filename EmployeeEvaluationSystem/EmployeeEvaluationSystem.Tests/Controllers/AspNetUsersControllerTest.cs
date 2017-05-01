using System;
using EmployeeEvaluationSystem.MVC.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using EmployeeEvaluationSystem.Entity;
using System.Linq;

namespace EmployeeEvaluationSystem.Tests.Controllers
{
    [TestClass]
    public class AspNetUsersControllerTest
    {
        [TestMethod]
        public void AspNetUserIndexNoUserTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create();



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            var view = aspUser.Index().Result;
            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(view);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<List<PersonalAspNetUserViewModel>>(routeResult.Model);


            UTH.IsTrue(
                modelResult.Count == 0
                );
        }

        [TestMethod]
        public void AspNetUserIndexThreeUserTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser(), new AspNetUser(), new AspNetUser()
            });



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            var view = aspUser.Index().Result;
            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(view);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<List<PersonalAspNetUserViewModel>>(routeResult.Model);


            UTH.IsTrue(
                modelResult.Count == 3
                );
        }

        [TestMethod]
        public void AspNetUserDetailsUserTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser(), new AspNetUser() { Id = "123" }, new AspNetUser()
            });



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            var view = aspUser.Details("123").Result;
            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(view);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<PersonalAspNetUserViewModel>(routeResult.Model);


            UTH.IsTrue(
                modelResult.Id == "123"
                );
        }


        [TestMethod]
        public void AspNetUserDetailsNoIdUserTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser(), new AspNetUser() { Id = "123" }, new AspNetUser()
            });



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            var view = aspUser.Details(null).Result;
            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<HttpStatusCodeResult>(view);
          
            UTH.IsTrue(
                routeResult.StatusCode == 400
                );
        }

        [TestMethod]
        public void AspNetUserDetailsNotFoundsUserTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser(), new AspNetUser() { Id = "123" }, new AspNetUser()
            });



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            var view = aspUser.Details("456").Result;
            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<HttpNotFoundResult>(view);

            UTH.IsTrue(
                routeResult.StatusCode == 404
                );
        }

        [TestMethod]
        public void AspNetGetEditDetailsUserTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser(), new AspNetUser() { Id = "123" }, new AspNetUser()
            });



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            var view = aspUser.Details("123").Result;
            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<ViewResult>(view);
            var modelResult = UTH.CheckIsTypeNotNullAndGetObj<PersonalAspNetUserViewModel>(routeResult.Model);


            UTH.IsTrue(
                modelResult.Id == "123"
                );
        }


        [TestMethod]
        public void AspNetUserGetEditNoIdUserTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser(), new AspNetUser() { Id = "123" }, new AspNetUser()
            });



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            var view = aspUser.Edit(null as string).Result;
            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<HttpStatusCodeResult>(view);

            UTH.IsTrue(
                routeResult.StatusCode == 400
                );
        }

        [TestMethod]
        public void AspNetUserGetEditNotFoundsUserTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser(), new AspNetUser() { Id = "123" }, new AspNetUser()
            });



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            var view = aspUser.Details("456").Result;
            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<HttpNotFoundResult>(view);

            UTH.IsTrue(
                routeResult.StatusCode == 404
                );
        }


        [TestMethod]
        public void AspNetPostEditDetailsUserTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser(), new AspNetUser() { Id = "123" }, new AspNetUser()
            });



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            var view = aspUser.Edit(new PersonalAspNetUserViewModel() { Id = "123", FirstName = "Tim" }).Result;

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<RedirectToRouteResult>(view);
            UTH.IsTrue(
                routeResult.RouteValues.Any(y => y.Key == "action" && y.Value.ToString() == "Index")
                );
        }

        [TestMethod]
        public void AspNetPostEditNotFoundUserTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser(), new AspNetUser() { Id = "456" }, new AspNetUser()
            });



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            try
            {
                var view = aspUser.Edit(new PersonalAspNetUserViewModel() { Id = "123", FirstName = "Tim" }).Result;
                Assert.Fail();
            }
            catch (Exception)
            {
                
            }
        }

        [TestMethod]
        public void AspNetPostEditDetailsNoUserModelTest()
        {

            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
            .Create()
            .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser(), new AspNetUser() { Id = "123" }, new AspNetUser()
            });



            AspNetUsersController aspUser = new AspNetUsersController(x.GetCreator());

            var view = aspUser.Edit(null as PersonalAspNetUserViewModel).Result;

            var routeResult = UTH.CheckIsTypeNotNullAndGetObj<HttpStatusCodeResult>(view);

            UTH.IsTrue(
                routeResult.StatusCode == 400
                );
        }


    }
}
