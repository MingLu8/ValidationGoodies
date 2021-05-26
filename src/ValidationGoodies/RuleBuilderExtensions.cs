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
        public static IRuleBuilderOptionsConditions<T, TProperty> ForProperty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<TProperty, object>> propertyExpression, Action<ChildPropertyRuleBuilder<T, TProperty>> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var propertyName = GetPropertyName<TProperty>(propertyExpression);

            return (IRuleBuilderOptionsConditions<T, TProperty>)ruleBuilder.Must((parent, value, context) => {
                var propertyValue = GetValue(propertyName, value);

                action(new ChildPropertyRuleBuilder<T, TProperty>(propertyName, propertyValue, value, context));
                return true;
            });
        }

        public static IRuleBuilderOptions<T, TProperty> MustAsync2<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<T, TProperty, ValidationContext<T>, CancellationToken, Task<bool>> predicate)
        {
            return ruleBuilder.SetAsyncValidator(new AsyncPredicateValidator<T, TProperty>(predicate));
        }

        public static IRuleBuilderOptionsConditions<T, TProperty> ForPropertyAsync<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<TProperty, object>> propertyExpression, Func<ChildPropertyRuleBuilder<T, TProperty>, CancellationToken, Task> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var propertyName = GetPropertyName<TProperty>(propertyExpression);

            async Task<bool> Func(T parent, TProperty value, ValidationContext<T> context, CancellationToken cancellationToken)
            {
                var propertyValue = GetValue(propertyName, value);
                await action(new ChildPropertyRuleBuilder<T, TProperty>(propertyName, propertyValue, value, context), cancellationToken);
                return true;
            }

            return (IRuleBuilderOptionsConditions<T, TProperty>)ruleBuilder.MustAsync((Func<T, TProperty, ValidationContext<T>, CancellationToken, Task<bool>>) Func);
        }

        private static object GetValue(string propertyName, object obj)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            return prop.GetValue(obj);
        }

        private static string GetPropertyName<TElement>(Expression<Func<TElement, object>> propertyExpression)
        {
            return (propertyExpression.Body as MemberExpression).Member.Name;
        }
    }
}