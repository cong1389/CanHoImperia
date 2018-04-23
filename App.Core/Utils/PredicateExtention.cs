using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace App.Core.Utils
{
	public class PredicateExtention
	{
		private static readonly MethodInfo StringContainsMethod;

		private static readonly MethodInfo BooleanEqualsMethod;

		static PredicateExtention()
		{
			StringContainsMethod = typeof(string).GetMethod("Contains", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null);
			BooleanEqualsMethod = typeof(bool).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(bool) }, null);
		}

	    private static Expression<Func<TDbType, bool>> ApplyBoolCriterion<TDbType, TSearchCriteria>(
		    TSearchCriteria searchCriteria, PropertyInfo searchCriterionPropertyInfo, Type dbType,
		    MemberInfo dbFieldMemberInfo, Expression<Func<TDbType, bool>> predicate)
		{
			Expression<Func<TDbType, bool>> expression;
		    if (searchCriterionPropertyInfo.GetValue(searchCriteria) is bool value)
			{
				ParameterExpression parameterExpression = Expression.Parameter(dbType, "x");
				MemberExpression memberExpression = Expression.MakeMemberAccess(parameterExpression, dbFieldMemberInfo);
				Expression[] expressionArray = { Expression.Constant(value) };
				MethodCallExpression methodCallExpression = Expression.Call(memberExpression, BooleanEqualsMethod, expressionArray);
				Expression<Func<TDbType, bool>> expression1 = Expression.Lambda(methodCallExpression, parameterExpression) as Expression<Func<TDbType, bool>>;
				expression = predicate.And(expression1);
			}
			else
			{
				expression = predicate;
			}
			return expression;
		}

		private static Expression<Func<TDbType, bool>> ApplyStringCriterion<TDbType, TSearchCriteria>(TSearchCriteria searchCriteria, PropertyInfo searchCriterionPropertyInfo, Type dbType, MemberInfo dbFieldMemberInfo, Expression<Func<TDbType, bool>> predicate)
		{
			Expression<Func<TDbType, bool>> expression;
			string value = searchCriterionPropertyInfo.GetValue(searchCriteria) as string;
			if (!string.IsNullOrWhiteSpace(value))
			{
				ParameterExpression parameterExpression = Expression.Parameter(dbType, "x");
				MemberExpression memberExpression = Expression.MakeMemberAccess(parameterExpression, dbFieldMemberInfo);
				Expression[] expressionArray = { Expression.Constant(value) };
				MethodCallExpression methodCallExpression = Expression.Call(memberExpression, StringContainsMethod, expressionArray);
				Expression<Func<TDbType, bool>> expression1 = Expression.Lambda(methodCallExpression, parameterExpression) as Expression<Func<TDbType, bool>>;
				expression = predicate.And(expression1);
			}
			else
			{
				expression = predicate;
			}
			return expression;
		}

		public static Expression<Func<TDbType, bool>> BuildPredicate<TDbType, TSearchCriteria>(TSearchCriteria searchCriteria)
		{
			Expression<Func<TDbType, bool>> expression = PredicateBuilder.True<TDbType>();
			PropertyInfo[] properties = searchCriteria.GetType().GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				string dbFieldName = GetDbFieldName(propertyInfo);
				Type type = typeof(TDbType);
				MemberInfo memberInfo = type.GetMember(dbFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public).Single();
				if (propertyInfo.PropertyType == typeof(string))
				{
					expression = ApplyStringCriterion(searchCriteria, propertyInfo, type, memberInfo, expression);
				}
				else if (propertyInfo.PropertyType == typeof(bool?))
				{
					expression = ApplyBoolCriterion(searchCriteria, propertyInfo, type, memberInfo, expression);
				}
			}
			return expression;
		}

		private static string GetDbFieldName(PropertyInfo propertyInfo)
		{
			object obj = propertyInfo.GetCustomAttributes(typeof(DbFieldMapAttribute), false).FirstOrDefault();
			return obj != null ? ((DbFieldMapAttribute)obj).Field : propertyInfo.Name;
		}
	}
}