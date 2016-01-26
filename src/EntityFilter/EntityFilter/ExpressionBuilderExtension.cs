using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFilter
{
    public static class ExpressionBuilderExtension
    {
        #region OrderBy 

        private static readonly MethodInfo OrderByMethod =
            typeof(Queryable).GetMethods()
                .Where(method => method.Name == "OrderBy")
                .Single(method => method.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescMethod =
            typeof(Queryable).GetMethods()
                .Where(method => method.Name == "OrderByDescending")
                .Single(method => method.GetParameters().Length == 2);

        /// <summary>
        /// Extension method to order a TSource entity using reflection
        /// <para>To order in ascending order, way must be equal to "asc"</para>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="way">ascending or descending direction</param>
        /// <returns></returns>
        public static IQueryable<TSource> OrderByExpressionBuilder<TSource>(this IQueryable<TSource> source,
            string propertyName,
            string way)
        {
            string[] props = propertyName.Split('.');

            Type type = typeof(TSource);

            ParameterExpression parameter = Expression.Parameter(type, "x");

            Expression orderByProperty = parameter;

            foreach (var prop in props)
            {
                try
                {
                    PropertyInfo info = type.GetProperty(prop);
                    orderByProperty = Expression.Property(orderByProperty, info);
                    type = info.PropertyType;
                }
                catch (Exception e)
                {
                    throw new ExpressionBuilderException($"The property {prop} does not exist for the type {type}", e);
                }
            }

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TSource), type);

            LambdaExpression lambda = Expression.Lambda(delegateType, orderByProperty, parameter);

            MethodInfo genericMethod = (way == "asc")
                ? OrderByMethod.MakeGenericMethod(typeof(TSource), type)
                : OrderByDescMethod.MakeGenericMethod(typeof(TSource), type);

            object result = genericMethod.Invoke(null, new object[] { source, lambda });

            return (IQueryable<TSource>)result;
        }

        #endregion

        #region Where

        /// <summary>
        /// Extension method to filter a TSource entity with Where clause using reflection 
        /// OperatorType specifies the expression to apply for the target property in the Where clause
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="model"></param>
        /// <param name="optr"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereExpressionBuilder<TSource, TValue, TModel>(
            this IQueryable<TSource> source,
            string propertyName, TValue value, TModel model, OperatorType optr)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), "a");

            Expression expression = GetNavigationPropertyExpression(parameter, propertyName, typeof(TSource), model);

            expression = expression.ApplyOperatorTypeToExpression(value, optr);

            LambdaExpression lambda = Expression.Lambda<Func<TSource, bool>>(expression, parameter);

            MethodInfo genericMethod =
                typeof(Queryable).GetMethods()
                    .First(method => method.Name == "Where")
                    .MakeGenericMethod(typeof(TSource));

            object result = genericMethod.Invoke(null, new object[] { source, lambda });

            return (IQueryable<TSource>)result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="p"></param>
        /// <param name="propertyName"></param>
        /// <param name="sourceType"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static Expression GetNavigationPropertyExpression<TModel>(ParameterExpression p, string propertyName,
            Type sourceType,
            TModel model)
        {
            string[] props = propertyName.Split('.');

            Expression expression = p;

            foreach (var prop in props)
            {
                if (IsCollectionProperty(prop))
                {
                    expression = expression.ParseCollectionToExpression(prop, sourceType, model);
                    sourceType = expression.Type;
                }
                else
                {
                    try
                    {
                        PropertyInfo info = sourceType.GetProperty(prop);
                        expression = Expression.Property(expression, info);
                        sourceType = info.PropertyType;
                    }
                    catch (ExpressionBuilderException e)
                    {
                        throw new ExpressionBuilderException(
                            $"The property {prop} does not exist for the type {sourceType}", e);
                    }
                }
            }

            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private static bool IsCollectionProperty(string prop)
        {
            return prop.StartsWith("[") && prop.EndsWith("]");
        }

        /// <summary>
        /// Tokenize the special formatted property 
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private static Tuple<string, string, string[]> Tokenizer(string prop)
        {
            var tokens = prop.Trim('[', ']').Split('|');

            if (tokens.Length == 3)
            {
                var compareTokens = tokens[2].Trim().Split(' ');

                if (compareTokens.Length != 3)
                    throw new ExpressionBuilderException($"Parsing error : {tokens[2]} must have 3 parameters : \n" +
                                                         "1 - The collection's property name \n" +
                                                         "2 - The operator to apply (==, >=, <=, !=) \n" +
                                                         "3 - The model's value to compare with");

                return new Tuple<string, string, string[]>(tokens[0].Trim(), tokens[1].Trim(), compareTokens);
            }

            throw new ExpressionBuilderException(
                $"Parsing error : {prop} must use the following format [1 | 2 | 3] : \n" +
                "1 - The collection name \n" +
                "2 - The LINQ method to apply \n" +
                "3 - The properties to compare (ex: Foo == Bar)");
        }


        private static readonly Dictionary<string, OperatorType> OperatorMatcherDictionnary = new Dictionary
            <string, OperatorType>
        {
            ["=="] = OperatorType.EQUAL,
            [">="] = OperatorType.GREATER_THAN_OR_EQUAL,
            ["<="] = OperatorType.LESS_THAN_OR_EQUAL,
            ["!="] = OperatorType.NOT_EQUAL
        };


        /// <summary>
        /// 
        /// </summary>
        /// <param name="oprToken"></param>
        /// <returns></returns>
        private static OperatorType OperatorMatcher(string oprToken)
        {
            if (!OperatorMatcherDictionnary.ContainsKey(oprToken))
                throw new ExpressionBuilderException($"{oprToken} is not a valid operator.");

            return OperatorMatcherDictionnary[oprToken];
        }


        /// <summary>
        /// Parse the special formatted property "[CollectionName | LINQMethod | CollectionProperty Operator ModelProperty]"
        /// Create a lambda expression to filter this collection
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="prop"></param>
        /// <param name="sourceType"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static Expression ParseCollectionToExpression<TModel>(this Expression expression, string prop,
            Type sourceType, TModel model)
        {
            var tokens = Tokenizer(prop);

            try
            {
                PropertyInfo info = sourceType.GetProperty(tokens.Item1);
                Type t = info.PropertyType;

                if (t.IsGenericType && typeof(ICollection<>).IsAssignableFrom(t.GetGenericTypeDefinition()) ||
                    t.GetInterfaces()
                        .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)))
                {
                    expression = Expression.Property(expression, info);
                    sourceType = info.PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    throw new ExpressionBuilderException(
                        $"The property {tokens.Item1} is not a valid collection in {sourceType}");
                }
            }
            catch (ExpressionBuilderException e)
            {
                throw new ExpressionBuilderException(
                    $"The property {tokens.Item1} does not exist for the type {sourceType}", e);
            }

            ParameterExpression nestedParam = Expression.Parameter(sourceType, "x");
            Expression nestedExpression = nestedParam;

            PropertyInfo nestedInfo = sourceType.GetProperty(tokens.Item3[0]);
            nestedExpression = Expression.Property(nestedExpression, nestedInfo);

            var modelProp = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                          BindingFlags.NonPublic)
                .FirstOrDefault(x => x.Name == tokens.Item3[2]);

            if (modelProp == null)
                throw new ExpressionBuilderException(
                    $"The property {tokens.Item3[2]} does not exist for the model {typeof(TModel)}");

            nestedExpression = nestedExpression.ApplyOperatorTypeToExpression(modelProp.GetValue(model, null),
                OperatorMatcher(tokens.Item3[1]));

            Type delegateType = typeof(Func<,>).MakeGenericType(sourceType, typeof(bool));

            LambdaExpression nestedLambda = Expression.Lambda(delegateType, nestedExpression, nestedParam);

            try
            {
                MethodInfo tokenMethod =
                    typeof(Enumerable).GetMethods()
                        .First(x => x.Name == tokens.Item2 && x.GetParameters().Length == 2)
                        .MakeGenericMethod(sourceType);

                expression = Expression.Call(tokenMethod, expression, nestedLambda);

                return expression;
            }
            catch (ExpressionBuilderException e)
            {
                throw new ExpressionBuilderException($"The LINQ method {tokens.Item2} does not exist", e);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <param name="optr"></param>
        /// <returns></returns>
        private static Expression ApplyOperatorTypeToExpression<TValue>(this Expression expression, TValue value,
            OperatorType optr)
        {
            switch (optr)
            {
                case OperatorType.EQUAL:
                    return Expression.Equal(expression, Expression.Constant(value));
                case OperatorType.NOT_EQUAL:
                    return Expression.NotEqual(expression, Expression.Constant(value));
                case OperatorType.GREATER_THAN:
                    return Expression.GreaterThan(expression, Expression.Constant(value));
                case OperatorType.LESS_THAN:
                    return Expression.LessThan(expression, Expression.Constant(value));
                case OperatorType.LESS_THAN_OR_EQUAL:
                    return Expression.LessThanOrEqual(expression, Expression.Constant(value));
                case OperatorType.GREATER_THAN_OR_EQUAL:
                    return Expression.GreaterThanOrEqual(expression, Expression.Constant(value));
                case OperatorType.CONTAINS:
                    Expression expressionNull = Expression.NotEqual(expression,
                        Expression.Constant(null, expression.Type));
                    MethodInfo methodContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    Expression expressionContains = Expression.Call(expression, methodContains,
                        Expression.Constant(value));
                    return Expression.AndAlso(expressionNull, expressionContains);
                default:
                    return Expression.Empty();
            }
        }

        #endregion
    }
}
