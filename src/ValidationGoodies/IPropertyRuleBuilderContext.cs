using System;
using System.Threading.Tasks;
using FluentValidation;

namespace ValidationGoodies
{
    public interface IPropertyRuleBuilderContext<T, TElement, TPropertyType> where TPropertyType : IComparable
    {
        bool NoCascade { get; }
        bool Failed { get; }
        string PropertyName { get; }
        TPropertyType PropertyValue { get; }
        TElement InstanceToValidate { get; }
        T ParentInstanceToValidate { get; }
        ValidationContext<T> Context { get; }
        PropertyRuleBuilderContext<T, TElement, TPropertyType> Cascade();
        PropertyRuleBuilderContext<T, TElement, TPropertyType> NotEmpty();
        PropertyRuleBuilderContext<T, TElement, TPropertyType> Max(TPropertyType max);
        PropertyRuleBuilderContext<T, TElement, TPropertyType> Min(TPropertyType min);
        PropertyRuleBuilderContext<T, TElement, TPropertyType> Length(int min, int max);
        PropertyRuleBuilderContext<T, TElement, TPropertyType> Must(Func<bool> func, string errorMessage);
        Task<PropertyRuleBuilderContext<T, TElement, TPropertyType>> MustAsync(Func<Task<bool>> func, string errorMessage);
    }
}