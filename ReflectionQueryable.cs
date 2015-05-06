using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BackendPartenaire.Extensions
{
    public static class ReflectionQueryable
    {
        private static readonly MethodInfo OrderByMethod =
            typeof(Queryable).GetMethods()
                .Where(method => method.Name == "OrderBy").Single(method => method.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescMethod = 
            typeof(Queryable).GetMethods()
                .Where(method => method.Name == "OrderByDescending").Single(method => method.GetParameters().Length == 2);

        /// <summary>
        /// Generic extension method to order a TSource entity based on reflexion
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="way">ascending or descending direction</param>
        /// <returns></returns>
        public static IQueryable<TSource> OrderByReflexion<TSource>(this IQueryable<TSource> source, string propertyName, string way)
        {
            string[] props = propertyName.Split('.');
            Type type = typeof(TSource);
            ParameterExpression parameter = Expression.Parameter(type, "x");
            Expression orderByProperty = parameter;

            foreach (var prop in props)
            {
                PropertyInfo info = type.GetProperty(prop);
                orderByProperty = Expression.Property(orderByProperty, info);
                type = info.PropertyType;
            }

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TSource), type);
            
            LambdaExpression lambda = Expression.Lambda(delegateType, orderByProperty, parameter);

            MethodInfo genericMethod = (way == "asc") ? OrderByMethod.MakeGenericMethod(typeof(TSource), type) : OrderByDescMethod.MakeGenericMethod(typeof(TSource), type);
            
            object result = genericMethod.Invoke(null, new object[] { source, lambda });

            return (IQueryable<TSource>)result;
        }


        /// <summary>
        /// Generic extension method to query a TSource entity with Where clause based on reflexion 
        /// OperatorType specifies the expression to apply in the Where clause
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="optr"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereReflexion<TSource, TValue>(this IQueryable<TSource> source, string propertyName, TValue value, OperatorType optr)
        {
            string[] props = propertyName.Split('.');
            Type type = typeof(TSource);
            ParameterExpression parameter = Expression.Parameter(type, "x");
            Expression whereProperty = parameter;

            try
            {
                foreach (var prop in props)
                {
                    PropertyInfo info = type.GetProperty(prop);
                    whereProperty = Expression.Property(whereProperty, info);
                    type = info.PropertyType;
                }
            }
            catch (Exception)
            {
                throw new Exception(String.Format("The property {0} does not exist for the type {1}", propertyName, type));
            }

            Expression body = null;
            
            switch (optr)
            {   
                case OperatorType.Equal:
                    body = Expression.Equal(whereProperty, Expression.Constant(value));
                    break;
                case OperatorType.EqualDate:
                    PropertyInfo date = typeof (DateTime).GetProperty("Date");
                    whereProperty = Expression.Property(whereProperty, date);
                    body = Expression.Equal(whereProperty, Expression.Constant(value));
                    break;
                case OperatorType.LessThanOrEqual:
                    body = Expression.LessThanOrEqual(whereProperty, Expression.Constant(value));
                    break;
                case OperatorType.GreaterThanOrEqual:
                    body = Expression.GreaterThanOrEqual(whereProperty, Expression.Constant(value));
                    break;
                case OperatorType.Contains:
                    Expression expressionNull = Expression.NotEqual(whereProperty,
                        Expression.Constant(null, whereProperty.Type));

                    MethodInfo methodContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    Expression expressionContains = Expression.Call(whereProperty, methodContains, Expression.Constant(value));

                    body = Expression.AndAlso(expressionNull, expressionContains);
                    break;
                default:
                    break;
            }

            LambdaExpression lambda = Expression.Lambda<Func<TSource, bool>>(body, parameter);

            MethodInfo genericMethod = typeof(Queryable).GetMethods().First(method => method.Name == "Where").MakeGenericMethod(typeof(TSource));

            object result = genericMethod.Invoke(null, new object[] { source, lambda });

            return (IQueryable<TSource>)result;
        }
    }
}