using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace ValidationGoodies
{
    public interface IPropertyRuleBuilder<T, TProperty, TPropertyType> where TPropertyType : IComparable
    {
        IRuleBuilder<T, TProperty> UseRules(Action<IPropertyRuleBuilderContext<T, TProperty, TPropertyType>> action);
        IRuleBuilder<T, TProperty> UseRulesAsync(Func<IPropertyRuleBuilderContext<T, TProperty, TPropertyType>, Task> action);
        IRuleBuilder<T, TProperty> UseRulesAsync(Func<IPropertyRuleBuilderContext<T, TProperty, TPropertyType>, CancellationToken, Task> action);
    }
}