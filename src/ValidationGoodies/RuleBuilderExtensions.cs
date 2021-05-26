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
        public static IRuleBuilderOptionsConditions<T, TProperty> ForProperty<T, TProperty, PropertyType>(this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<TProperty, PropertyType>> propertyExpression, Action<ChildPropertyRuleBuilder<T, TProperty, PropertyType>> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var propertyName = GetPropertyName<TProperty, PropertyType>(propertyExpression);

            return (IRuleBuilderOptionsConditions<T, TProperty>)ruleBuilder.Must((parent, value, context) => {
                var propertyValue = GetValue<PropertyType>(propertyName, value);

                action(new ChildPropertyRuleBuilder<T, TProperty, PropertyType>(propertyName, propertyValue, value, context));
                return true;
            });
        }

        public static IRuleBuilderOptionsConditions<T, TProperty> ForPropertyAsync<T, TProperty, PropertyType>(this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<TProperty, PropertyType>> propertyExpression, Func<ChildPropertyRuleBuilder<T, TProperty, PropertyType>, CancellationToken, Task> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var propertyName = GetPropertyName<TProperty, PropertyType>(propertyExpression);

            async Task<bool> Func(T parent, TProperty value, ValidationContext<T> context, CancellationToken cancellationToken)
            {
                var propertyValue = GetValue<PropertyType>(propertyName, value);
                await action(new ChildPropertyRuleBuilder<T, TProperty, PropertyType>(propertyName, propertyValue, value, context), cancellationToken);
                return true;
            }

            return (IRuleBuilderOptionsConditions<T, TProperty>)ruleBuilder.MustAsync((Func<T, TProperty, ValidationContext<T>, CancellationToken, Task<bool>>) Func);
        }

        private static PropertyType GetValue<PropertyType>(string propertyName, object obj)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            
            dynamic val = prop.GetValue(obj);
            return val;
        }

        private static string GetPropertyName<TElement, PropertyType>(Expression<Func<TElement, PropertyType>> propertyExpression)
        {
            return (propertyExpression.Body as MemberExpression).Member.Name;
        }
    }
}