using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeEvaluationSystem.Tests.Infrastructure
{
    public class EFUnitOfWorkBuilder<T> : IUnitOfWorkBuilder<T> where T : DbContext
    {

        private Mock<T> mockContext { get; set; }

        public EFUnitOfWorkBuilder()
        {
            mockContext = new Mock<T>();

            Initialize();
        }

        private void Initialize()
        {
            var tType = typeof(T);

            //var theProps = tType.GetProperties()?.Where(x => typeof(DbSet<>).IsAssignableFrom(x.PropertyType)).ToList();

            var theProps = tType.GetProperties()?.Where(x => x.PropertyType.IsGenericType && (x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))).ToList();

            foreach (var dbProps in theProps)
            {

                var type = dbProps.PropertyType.GetGenericArguments().FirstOrDefault();

                if (type == null)
                {
                    continue;
                }

                //Type genericListType = typeof(List<>).MakeGenericType(type);
                //var obj = (IList)Activator.CreateInstance(genericListType);

                //var queriableObj = obj.AsQueryable();

                //Type dbSetGeneric = typeof(DbSet<>).MakeGenericType(type);

                //Type theMockGeneric = typeof(Mock<>).MakeGenericType(dbSetGeneric);

                //var mockSet = (Mock)Activator.CreateInstance(theMockGeneric);

                //mockSet.As<IQueryable>().Setup(m => m.Provider).Returns(queriableObj.Provider);
                //mockSet.As<IQueryable>().Setup(m => m.Expression).Returns(queriableObj.Expression);
                //mockSet.As<IQueryable>().Setup(m => m.ElementType).Returns(queriableObj.ElementType);
                //mockSet.As<IQueryable>().Setup(m => m.GetEnumerator()).Returns(queriableObj.GetEnumerator());

                ParameterExpression param = Expression.Parameter(typeof(T), "context");

                var memExp = Expression.PropertyOrField(param, dbProps.Name);

                Type fakeEnumerableType = typeof(IEnumerable<>).MakeGenericType(type);

                var theFuncType = typeof(Func<,>).MakeGenericType(typeof(T), fakeEnumerableType);

                var expressionType = typeof(Expression<>).MakeGenericType(theFuncType);

                var newMI = typeof(Expression).GetGenericMethod("Lambda", new Type[] { typeof(Expression), typeof(ParameterExpression[]) });

                MethodInfo genericMethod = newMI.MakeGenericMethod(theFuncType);
                
                var lambda = genericMethod.Invoke(null, new object[] { memExp, new ParameterExpression[] { param } });

                MethodInfo mi = this.GetType().GetMethod( nameof(this.InitializeOne), BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                mi = mi.MakeGenericMethod(type);

                mi.Invoke(this, new object[] { lambda, null });


            }
        }

        public void InitializeOne<T3>(Expression<Func<T, IEnumerable<T3>>> x, IList<T3> data = null) where T3 : class
        {

            if(data == null)
            {
                data = new List<T3>();
            }

            var newData = data.AsQueryable();

            var mockSet = new Mock<DbSet<T3>>();
            mockSet.As<IQueryable<T3>>().Setup(m => m.Provider).Returns(newData.Provider);
            mockSet.As<IQueryable<T3>>().Setup(m => m.Expression).Returns(newData.Expression);
            mockSet.As<IQueryable<T3>>().Setup(m => m.ElementType).Returns(newData.ElementType);
            mockSet.As<IQueryable<T3>>().Setup(m => m.GetEnumerator()).Returns(newData.GetEnumerator());

            
            mockContext.Setup(x).Returns(mockSet.Object);
        }

        public T GetContext()
        {
            return this.mockContext.Object;
        }

        
    }

    public static class TypeExtensions
    {
        private class SimpleTypeComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return x.Assembly == y.Assembly &&
                    x.Namespace == y.Namespace &&
                    x.Name == y.Name;
            }

            public int GetHashCode(Type obj)
            {
                throw new NotImplementedException();
            }
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, Type[] parameterTypes)
        {
            var methods = type.GetMethods();
            foreach (var method in methods.Where(m => m.Name == name))
            {
                var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                if (methodParameterTypes.SequenceEqual(parameterTypes, new SimpleTypeComparer()))
                {
                    return method;
                }
            }

            return null;
        }
    }
}
