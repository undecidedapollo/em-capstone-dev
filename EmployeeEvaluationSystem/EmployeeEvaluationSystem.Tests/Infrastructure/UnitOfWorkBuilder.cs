using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

            var mockEntry = new Mock<DbEntityEntry>();
            //mockContext.Setup(y => y.Entry(It.IsAny<object>())).Returns(mockEntry.Object);


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

        public IUnitOfWorkBuilder<T> InitializeOne<T3>(Expression<Func<T, IEnumerable<T3>>> x, IList<T3> data = null) where T3 : class
        {

            if (data == null)
            {
                data = new List<T3>();
            }

            var newData = data.AsQueryable();

            var mockSet = new Mock<DbSet<T3>>();
            mockSet.As<IQueryable<T3>>().Setup(m => m.Provider).Returns(() => { return data.AsQueryable().Provider; });
            mockSet.As<IQueryable<T3>>().Setup(m => m.Expression).Returns(() => { return data.AsQueryable().Expression; });
            mockSet.As<IQueryable<T3>>().Setup(m => m.ElementType).Returns(() => { return data.AsQueryable().ElementType; });
            mockSet.As<IQueryable<T3>>().Setup(m => m.GetEnumerator()).Returns(() => { return data.AsQueryable().GetEnumerator(); });
            mockSet.As<IEnumerable<T3>>().Setup(m => m.GetEnumerator()).Returns(() => { return data.GetEnumerator(); });
            mockSet.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(() => { return data.GetEnumerator(); });

            mockSet.As<IDbSet<T3>>().Setup(m => m.Add(It.IsAny<T3>())).Returns<T3>((a) => { data.Add(a); return a; });
            mockSet.As<IDbSet<T3>>().Setup(m => m.Remove(It.IsAny<T3>())).Returns<T3>((a) => { data.Remove(a); return a; });
            mockSet.As<IDbSet<T3>>().Setup(m => m.Attach(It.IsAny<T3>())).Returns<T3>((a) => { data.Add(a); return a; });
            mockSet.As<IDbSet<T3>>().Setup(m => m.Find(It.IsAny<object[]>())).Throws(new NotImplementedException("Find has not yet been implemented."));


            var mockSetObj = mockSet.Object;

            

            mockContext.Setup(x).Returns(mockSetObj);
            mockContext.Setup(y => y.Set<T3>()).Returns(mockSetObj);

            

            return this;
        }

        public T GetContext()
        {
            return this.mockContext.Object;
        }
        public static IUnitOfWorkBuilder<T> Create()
        {
            return new EFUnitOfWorkBuilder<T>();
        }
    }

    public static class EFUnitOfWorkBuilder
    {

        public static IUnitOfWorkBuilder<TStatic> Create<TStatic>() where TStatic : DbContext
        {
            return new EFUnitOfWorkBuilder<TStatic>();
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
