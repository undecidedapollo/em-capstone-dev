using EmployeeEvaluationSystem.Entity;
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
        public void ThreeUserHasThreeUsers()
        {

            var x = new EFUnitOfWorkBuilder<EmployeeDatabaseEntities>();

            x.InitializeOne(y => y.AspNetUsers, new List<AspNetUser>
            {
                new AspNetUser { FirstName = "BBB", LastName = "LLL", UserName = "123" },
                new AspNetUser { FirstName = "CCC", LastName = "LLL", UserName = "456" },
                new AspNetUser { FirstName = "DDD", LastName = "LLL", UserName = "789" }
            });

            using(var unitOfWork = new UnitOfWork(x.GetContext()))
            {
                var users = unitOfWork.Users.GetAllUsers(null).ToList();

                unitOfWork.Complete();

                Assert.AreEqual(3, users.Count());
                Assert.AreEqual("BBB", users[0].FirstName);
                Assert.AreEqual("CCC", users[1].FirstName);
                Assert.AreEqual("DDD", users[2].FirstName);
            }
        }

       
    }
}
