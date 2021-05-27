using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;

namespace ValidationGoodies
{
    public static class RuleBuilderExtensions
    {
        public static IPropertyRuleBuilder<T, TProperty, TPropertyType> ForProperty<T, TProperty, TPropertyType>(this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<TProperty, TPropertyType>> propertyExpression) where TPropertyType : IComparable
        {
            var propertyName = GetPropertyName<TProperty, TPropertyType>(propertyExpression);
            return new PropertyRuleBuilder<T, TProperty, TPropertyType>(ruleBuilder, propertyName);
        }

        private static string GetPropertyName<TElement, TPropertyType>(Expression<Func<TElement, TPropertyType>> propertyExpression)
        {
            return (propertyExpression.Body as MemberExpression).Member.Name;
        }
    }
}