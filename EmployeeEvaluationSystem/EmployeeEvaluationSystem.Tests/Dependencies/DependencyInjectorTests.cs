using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeEvaluationSystem.Entity.SharedObjects.Infrastructure;

namespace EmployeeEvaluationSystem.Tests.Dependencies
{
    [TestClass]
    public class DependencyInjectorTests
    {
        public object testLock = new object();




        [TestMethod]
        public void DependencyGetInstanceReturnsInstance()
        {
            lock (testLock)
            {
                var result = Dependency.GetInstance();

                Dependency.ClearDependencies();

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(Dependency));
            }

        }

        [TestMethod]
        public void DependencyFindNoInstanceThrowsException()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                try
                {
                    var result = Dependency.FindMy<List<int>>();
                    Assert.Fail();
                }
                catch (Exception)
                {
                    Assert.IsTrue(true);
                }
            }
          
        }

        [TestMethod]
        public void DependencyRegisterInstanceReturnsInstance()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                Dependency.RegisterInstance(() => new List<int>());

                var result = Dependency.FindMy<List<int>>();

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(List<int>));
            }
           
        }

        [TestMethod]
        public void DependencyRegisterInterfaceReturnsDifferentObject()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                Dependency.RegisterInstance(() => new List<int>());

                var result1 = Dependency.FindMy<List<int>>();
                var result2 = Dependency.FindMy<List<int>>();

                Assert.IsNotNull(result1);
                Assert.IsInstanceOfType(result1, typeof(List<int>));
                Assert.IsNotNull(result2);
                Assert.IsInstanceOfType(result2, typeof(List<int>));
                Assert.AreNotEqual(result1, result2);
            }
           
        }

        [TestMethod]
        public void DependencyRegisterInstanceInterfaceReturnsInterfaceInstance()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                Dependency.RegisterInstance<IEnumerable<int>>(() => new List<int>());

                var result = Dependency.FindMy<IEnumerable<int>>();

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(IEnumerable<int>));
            }
        }

        

        [TestMethod]
        public void DependencyRegisterInstanceNullFunctionThrowsException()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                try
                {
                    Dependency.RegisterInstance<List<int>>(null);
                    Assert.Fail();
                }
                catch (Exception)
                {
                    Assert.IsTrue(true);
                }
            }
           
        }

        [TestMethod]
        public void DependencyRegisterSynchronousInstanceReturnsInstance()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                Dependency.RegisterSynchronousInstance(() => new List<int>());

                var result = Dependency.FindMy<List<int>>();

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(List<int>));
            }

           
        }

        [TestMethod]
        public void DependencyRegisterSynchronousInstanceReturnsDifferentObject()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                Dependency.RegisterSynchronousInstance(() => new List<int>());

                var result1 = Dependency.FindMy<List<int>>();
                var result2 = Dependency.FindMy<List<int>>();

                Assert.IsNotNull(result1);
                Assert.IsInstanceOfType(result1, typeof(List<int>));
                Assert.IsNotNull(result2);
                Assert.IsInstanceOfType(result2, typeof(List<int>));
                Assert.AreNotEqual(result1, result2);
            }
           
        }

        [TestMethod]
        public void DependencyRegisterSynchronousInstanceInterfaceReturnsInterfaceInstance()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                Dependency.RegisterSynchronousInstance<IEnumerable<int>>(() => new List<int>());

                var result = Dependency.FindMy<IEnumerable<int>>();

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(IEnumerable<int>));
            }

           
        }

        [TestMethod]
        public void DependencyRegisterSynchronousInstanceNullFunctionThrowsException()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                try
                {
                    Dependency.RegisterSynchronousInstance<List<int>>(null);
                    Assert.Fail();
                }
                catch (Exception)
                {
                    Assert.IsTrue(true);
                }
            }
           
        }

        [TestMethod]
        public void DependencyRegisterSingletonReturnsInstance()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                Dependency.RegisterSingleton(() => new List<int>());

                var result = Dependency.FindMy<List<int>>();

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(List<int>));
            }
         
        }

        [TestMethod]
        public void DependencyRegisterSingletonReturnsSameObject()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                Dependency.RegisterSingleton(() => new List<int>());

                var result1 = Dependency.FindMy<List<int>>();
                var result2 = Dependency.FindMy<List<int>>();

                Assert.IsNotNull(result1);
                Assert.IsInstanceOfType(result1, typeof(List<int>));
                Assert.IsNotNull(result2);
                Assert.IsInstanceOfType(result2, typeof(List<int>));
                Assert.AreEqual(result1, result2);
            }
            
        }

        [TestMethod]
        public void DependencyRegisterSingletonReturnsInterfaceInstance()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                Dependency.RegisterSingleton<IEnumerable<int>>(() => new List<int>());

                var result = Dependency.FindMy<IEnumerable<int>>();

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(IEnumerable<int>));
            }

           
        }

        [TestMethod]
        public void DependencyRegisterSingletonFunctionThrowsException()
        {
            lock (testLock)
            {
                Dependency.ClearDependencies();
                try
                {
                    Dependency.RegisterSingleton<List<int>>(null);
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
