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
        public static IRuleBuilderOptionsConditions<T, TProperty> ForProperty<T, TProperty, TPropertyType>(this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<TProperty, TPropertyType>> propertyExpression, Action<ChildPropertyRuleBuilder<T, TProperty, TPropertyType>> action) where TPropertyType : IComparable
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var propertyName = GetPropertyName<TProperty, TPropertyType>(propertyExpression);

            return (IRuleBuilderOptionsConditions<T, TProperty>)ruleBuilder.Must((parent, value, context) => {
                var propertyValue = GetValue<TPropertyType>(propertyName, value);

                action(new ChildPropertyRuleBuilder<T, TProperty, TPropertyType>(propertyName, propertyValue, value, context));
                return true;
            });
        }

        public static IRuleBuilderOptionsConditions<T, TProperty> ForPropertyAsync<T, TProperty, TPropertyType>(this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<TProperty, TPropertyType>> propertyExpression, Func<ChildPropertyRuleBuilder<T, TProperty, TPropertyType>, CancellationToken, Task> action) where TPropertyType : IComparable
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var propertyName = GetPropertyName<TProperty, TPropertyType>(propertyExpression);

            async Task<bool> Func(T parent, TProperty value, ValidationContext<T> context, CancellationToken cancellationToken)
            {
                var propertyValue = GetValue<TPropertyType>(propertyName, value);
                await action(new ChildPropertyRuleBuilder<T, TProperty, TPropertyType>(propertyName, propertyValue, value, context), cancellationToken);
                return true;
            }

            return (IRuleBuilderOptionsConditions<T, TProperty>)ruleBuilder.MustAsync((Func<T, TProperty, ValidationContext<T>, CancellationToken, Task<bool>>) Func);
        }

        private static TPropertyType GetValue<TPropertyType>(string propertyName, object obj)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            var val = prop.GetValue(obj);
            if (val == null) return default(TPropertyType);
            if (val is TPropertyType value) return value;
            throw new Exception($"cannot convert property value to {typeof(TPropertyType).Name}");
        }

        private static string GetPropertyName<TElement, TPropertyType>(Expression<Func<TElement, TPropertyType>> propertyExpression)
        {
            return (propertyExpression.Body as MemberExpression).Member.Name;
        }
    }
}