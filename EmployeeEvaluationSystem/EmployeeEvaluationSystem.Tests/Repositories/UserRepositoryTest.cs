using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Authentication;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Tests.Repositories
{
    [TestClass]
    public class UserRepositoryTest
    {

        [TestMethod]
        public void GetAllUsers3UsersReturns3Users()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser { FirstName = "BBB", LastName = "LLL", UserName = "123" },
                    new AspNetUser { FirstName = "CCC", LastName = "LLL", UserName = "456" },
                    new AspNetUser { FirstName = "DDD", LastName = "LLL", UserName = "789" }
                });

            using(var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var users = unitOfWork.Users.GetAllUsers(null).ToList();
                

                Assert.AreEqual(3, users.Count());
                Assert.AreEqual("BBB", users[0].FirstName);
                Assert.AreEqual("CCC", users[1].FirstName);
                Assert.AreEqual("DDD", users[2].FirstName);
            }
        }

        [TestMethod]
        public void GetAllUsers0UsersReturns0Users()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create();

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var users = unitOfWork.Users.GetAllUsers(null).ToList();

                unitOfWork.Complete();

                Assert.AreEqual(0, users.Count());
                Assert.IsNull(users.FirstOrDefault());

            }
        }


        [TestMethod]
        public void GetNumberOfUsers3is3()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser { FirstName = "BBB", LastName = "LLL", UserName = "123" },
                    new AspNetUser { FirstName = "CCC", LastName = "LLL", UserName = "456" },
                    new AspNetUser { FirstName = "DDD", LastName = "LLL", UserName = "789" }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var userCount = unitOfWork.Users.GetNumberOfUsers();

                Assert.AreEqual(3, userCount);
            }
        }

        [TestMethod]
        public void GetNumberOfUsers0Is0()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create();

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var userCount = unitOfWork.Users.GetNumberOfUsers();

                Assert.AreEqual(0, userCount);
            }
        }

        [TestMethod]
        public void GetUserByIdIsCorrect()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser {Id="123", FirstName = "BBB", LastName = "LLL", UserName = "123" },
                    new AspNetUser {Id="456", FirstName = "CCC", LastName = "LLL", UserName = "456" },
                    new AspNetUser {Id="789", FirstName = "DDD", LastName = "LLL", UserName = "789" }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var theUser = unitOfWork.Users.GetUser("123", "123");

                Assert.IsNotNull(theUser);
                Assert.AreEqual(theUser.FirstName, "BBB");
                Assert.AreEqual(theUser.UserName, "123");
                Assert.AreEqual(theUser.Id, "123");

            }
        }

        [TestMethod]
        public void GetUserByIdIfNullIsNull()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser {Id="123", FirstName = "BBB", LastName = "LLL", UserName = "123" },
                    new AspNetUser {Id="456", FirstName = "CCC", LastName = "LLL", UserName = "456" },
                    new AspNetUser {Id="789", FirstName = "DDD", LastName = "LLL", UserName = "789" }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var theUser = unitOfWork.Users.GetUser("123", "555");

                Assert.IsNull(theUser);
            }
        }


        [TestMethod]
        public void EditUserEditsAllFields()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser {Id="123", FirstName = "BBB", LastName = "LLL", UserName = "123", MailingAddress="OLDADDRESS", PhoneNumber="OLDPHONE" },
                    new AspNetUser {Id="456", FirstName = "CCC", LastName = "LLL", UserName = "456" },
                    new AspNetUser {Id="789", FirstName = "DDD", LastName = "LLL", UserName = "789" }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var theUser = unitOfWork.Users.GetUser("123", "123");

                Assert.IsNotNull(theUser);

                var newUser = PersonalAspNetUserViewModel.Convert(theUser);

                Assert.IsNotNull(newUser);

                newUser.FirstName = "NEWFIRST";
                newUser.LastName = "NEWLAST";
                newUser.MailingAddress = "NEWMAIL";
                newUser.PhoneNumber = "NEWPHONE";
                newUser.EmployeeID = "NEWID";

                unitOfWork.Users.EditUser("123", newUser);

                var finalUser = unitOfWork.Users.GetUser("123", "123");

                Assert.IsNotNull(finalUser);
                Assert.AreEqual(finalUser.FirstName, newUser.FirstName);
                Assert.AreEqual(finalUser.LastName, newUser.LastName);
                Assert.AreEqual(finalUser.MailingAddress, newUser.MailingAddress);
                Assert.AreEqual(finalUser.PhoneNumber, newUser.PhoneNumber);
                Assert.AreEqual(finalUser.EmployeeID, newUser.EmployeeID);


            }
        }

        [TestMethod]
        public void EditUserThrowsExceptionIfUserNotFoundAndOriginalIsNotModified()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser {Id="123", FirstName = "BBB", LastName = "LLL", UserName = "123" },
                    new AspNetUser {Id="456", FirstName = "CCC", LastName = "LLL", UserName = "456" },
                    new AspNetUser {Id="789", FirstName = "DDD", LastName = "LLL", UserName = "789" }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var theUser = unitOfWork.Users.GetUser("123", "123");

                Assert.IsNotNull(theUser);

                var newUser = PersonalAspNetUserViewModel.Convert(theUser);

                Assert.IsNotNull(newUser);

                newUser.Id = "6785309";
                newUser.FirstName = "BLAH";

                try
                {
                    unitOfWork.Users.EditUser("123", newUser);
                    Assert.Fail();
                }
                catch (Exception)
                {
                    
                }

                var finalUser = unitOfWork.Users.GetUser("123", "123");

                Assert.IsNotNull(finalUser);
                Assert.AreEqual(finalUser.FirstName, "BBB");
            }
        }

        [TestMethod]
        public void EditUserThrowsExceptionIncomingItemIsNull()
        {
            var x = EFUnitOfWorkBuilder<EmployeeDatabaseEntities>
                .Create()
                .InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
                {
                    new AspNetUser {Id="123", FirstName = "BBB", LastName = "LLL", UserName = "123" },
                    new AspNetUser {Id="456", FirstName = "CCC", LastName = "LLL", UserName = "456" },
                    new AspNetUser {Id="789", FirstName = "DDD", LastName = "LLL", UserName = "789" }
                });

            using (var unitOfWork = new UnitOfWork(x.GetContext()))
            {

                try
                {
                    unitOfWork.Users.EditUser("123", null);
                    Assert.Fail();
                }
                catch (Exception)
                {
                    Assert.IsTrue(true);
                }
            }
        }


    }
}
