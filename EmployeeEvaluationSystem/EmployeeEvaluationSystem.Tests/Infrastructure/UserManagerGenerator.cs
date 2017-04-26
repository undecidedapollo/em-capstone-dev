using EmployeeEvaluationSystem.MVC;
using EmployeeEvaluationSystem.MVC.Models;
using Microsoft.AspNet.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Tests.Infrastructure
{
    public static class UserManagerGenerator
    {
        public static ApplicationUserManager GenerateManager(UserManagerOptions options)
        {
            var mockSet = new Mock<IUserStore<ApplicationUser>>();
            mockSet.Setup(m => m.FindByIdAsync(It.IsAny<string>())).Returns(
                (string x) => Task.FromResult(options?.Users?.FirstOrDefault(y => y.Id == x))
                );
            mockSet.Setup(m => m.FindByNameAsync(It.IsAny<string>())).Returns(
                (string x) => Task.FromResult(options?.Users?.FirstOrDefault(y => y.FirstName == x))
                );
            mockSet.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>())).Callback(
                (ApplicationUser x) => options?.Users?.Add(x));
            mockSet.Setup(m => m.DeleteAsync(It.IsAny<ApplicationUser>())).Callback(
                (ApplicationUser x) => options?.Users?.Remove(x));
            mockSet.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).Callback(
                (ApplicationUser x) => options.Users[options?.Users?.IndexOf(options?.Users.FirstOrDefault(z => z.Id == x.Id)) ?? 0] = x);

            var userManager = new ApplicationUserManager(mockSet.Object);

            return userManager;
        }
    }

    public class UserManagerOptions
    {
        public List<ApplicationUser> Users { get; set; }
    }
}
