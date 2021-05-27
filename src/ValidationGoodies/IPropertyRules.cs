using System;
using System.Threading.Tasks;
using FluentValidation;

namespace ValidationGoodies
{
    public interface IPropertyRules<T, TElement, TPropertyType> where TPropertyType : IComparable
    {
        bool NoCascade { get; }
        bool Failed { get; }
        string PropertyName { get; }
        TPropertyType PropertyValue { get; }
        TElement InstanceToValidate { get; }
        T ParentInstanceToValidate { get; }
        ValidationContext<T> Context { get; }
        PropertyRules<T, TElement, TPropertyType> Cascade();
        PropertyRules<T, TElement, TPropertyType> NotEmpty();
        PropertyRules<T, TElement, TPropertyType> Max(TPropertyType max);
        PropertyRules<T, TElement, TPropertyType> Min(TPropertyType min);
        PropertyRules<T, TElement, TPropertyType> Length(int min, int max);
        PropertyRules<T, TElement, TPropertyType> Length(int exactValue);
        PropertyRules<T, TElement, TPropertyType> Must(Func<bool> func, string errorMessage);
        Task<PropertyRules<T, TElement, TPropertyType>> MustAsync(Func<Task<bool>> func, string errorMessage);
    }
}