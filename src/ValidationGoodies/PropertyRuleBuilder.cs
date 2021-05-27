using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace ValidationGoodies
{
    public class PropertyRuleBuilder<T, TProperty, TPropertyType> : IPropertyRuleBuilder<T, TProperty, TPropertyType> where TPropertyType : IComparable
    {
        public string PropertyName { get; }
        private readonly IRuleBuilder<T, TProperty> _ruleBuilder;

        public PropertyRuleBuilder(IRuleBuilder<T, TProperty> ruleBuilder, string propertyName)
        {
            PropertyName = propertyName;
            _ruleBuilder = ruleBuilder;
        }

        public virtual IRuleBuilder<T, TProperty> UseRulesAsync(Func<IPropertyRules<T, TProperty, TPropertyType>, Task> action)
        {
            async Task<bool> Func(T parent, TProperty value, ValidationContext<T> context, CancellationToken cancellationToken)
            {
                var propertyValue = GetPropertyValue(value);
                await action(new PropertyRules<T, TProperty, TPropertyType>(PropertyName, propertyValue, value, context));
                return true;
            }

            return _ruleBuilder.MustAsync((Func<T, TProperty, ValidationContext<T>, CancellationToken, Task<bool>>)Func);
        }

        public virtual IRuleBuilder<T, TProperty> UseRulesAsync(Func<IPropertyRules<T, TProperty, TPropertyType>, CancellationToken, Task> action)
        {
            async Task<bool> Func(T parent, TProperty value, ValidationContext<T> context, CancellationToken cancellationToken)
            {
                var propertyValue = GetPropertyValue(value);
                await action(new PropertyRules<T, TProperty, TPropertyType>(PropertyName, propertyValue, value, context), cancellationToken);
                return true;
            }

            return _ruleBuilder.MustAsync((Func<T, TProperty, ValidationContext<T>, CancellationToken, Task<bool>>)Func);
        }

        public virtual IRuleBuilder<T, TProperty> UseRules(Action<IPropertyRules<T, TProperty, TPropertyType>> action)
        {
            return _ruleBuilder.Must((parent, value, context) => {
                var propertyValue = GetPropertyValue(value);

                action(new PropertyRules<T, TProperty, TPropertyType>(PropertyName, propertyValue, value, context));
                return true;
            });
        }


        protected virtual TPropertyType GetPropertyValue(object obj)
        {
            var prop = obj.GetType().GetProperty(PropertyName);
            var val = prop.GetValue(obj);
            if (val == null) return default(TPropertyType);
            if (val is TPropertyType value) return value;
            throw new Exception($"cannot convert property value to {typeof(TPropertyType).Name}");
        }
    }
}