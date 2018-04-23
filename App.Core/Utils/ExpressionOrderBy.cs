using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace App.Core.Utils
{
    public static class ExpressionOrderBy
	{
		private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
		{
			string[] strArrays = property.Split('.');
			Type propertyType = typeof(T);
			ParameterExpression parameterExpression = Expression.Parameter(propertyType, "x");
			Expression expression = parameterExpression;
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				PropertyInfo propertyInfo = propertyType.GetProperty(strArrays1[i]);
				expression = Expression.Property(expression, propertyInfo);
				propertyType = propertyInfo.PropertyType;
			}
			Type type = typeof(Func<,>).MakeGenericType(typeof(T), propertyType);
			LambdaExpression lambdaExpression = Expression.Lambda(type, expression, parameterExpression);
			return (IOrderedQueryable<T>) typeof(Queryable).GetMethods()
			    .Single(method =>
			        method.Name == methodName && method.IsGenericMethodDefinition && method.GetGenericArguments().Length == 2 &&
			        method.GetParameters().Length == 2).MakeGenericMethod(typeof(T), propertyType)
			    .Invoke(null, new object[] {source, lambdaExpression});
		}

		public static object GetPropertyValue(object obj, string property)
		{
			var propertyInfo = obj.GetType().GetProperty(property);

			return propertyInfo.GetValue(obj, null);
		}

		public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
		{
			return ApplyOrder(source, property, "OrderBy");
		}

		public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
		{
			return ApplyOrder(source, property, "OrderByDescending");
		}

		public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
		{
			return ApplyOrder(source, property, "ThenBy");
		}

		public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
		{
			return ApplyOrder(source, property, "ThenByDescending");
		}
	}
}