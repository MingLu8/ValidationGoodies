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
        IPropertyRules<T, TElement, TPropertyType> Cascade();
        IPropertyRules<T, TElement, TPropertyType> NotEmpty() => NotEmpty("must not be empty.");
        IPropertyRules<T, TElement, TPropertyType> NotEmpty(string errorMessage);
        IPropertyRules<T, TElement, TPropertyType> Max(TPropertyType max) => Max(max, $"cannot be greater than {max}, You entered {PropertyValue}.");
        IPropertyRules<T, TElement, TPropertyType> Max(TPropertyType max, string errorMessage);
        IPropertyRules<T, TElement, TPropertyType> Min(TPropertyType min) => Min(min, $"cannot be less than {min}, You entered {PropertyValue}.");
        IPropertyRules<T, TElement, TPropertyType> Min(TPropertyType min, string errorMessage);
        IPropertyRules<T, TElement, TPropertyType> Length(int min, int max) => Length(min, max, $"must be between {min} and {max} characters. You entered {PropertyValue?.ToString().Length ?? 0} characters.");
        IPropertyRules<T, TElement, TPropertyType> Length(int min, int max, string errorMessage);
        IPropertyRules<T, TElement, TPropertyType> Length(int exactValue) => Length(exactValue, $"must be exactly {exactValue} characters. You entered {PropertyValue?.ToString().Length ?? 0} characters.");
        IPropertyRules<T, TElement, TPropertyType> Length(int exactValue, string errorMessage);
        IPropertyRules<T, TElement, TPropertyType> MaxLength(int max) => MaxLength(max, $"must not be more than {max} characters. You entered {PropertyValue?.ToString().Length ?? 0} characters.");
        IPropertyRules<T, TElement, TPropertyType> MaxLength(int max, string errorMessage);
        IPropertyRules<T, TElement, TPropertyType> MinLength(int min) => MinLength(min, $"must not be less than {min} characters. You entered {PropertyValue?.ToString().Length ?? 0} characters.");
        IPropertyRules<T, TElement, TPropertyType> MinLength(int min, string errorMessage);
        IPropertyRules<T, TElement, TPropertyType> Matches(string expression) => Matches(expression, "has invalid format value.");
        IPropertyRules<T, TElement, TPropertyType> Matches(string expression, string errorMessage);
        IPropertyRules<T, TElement, TPropertyType> Must(Func<bool> func) => Must(func, "is invalid.");
        IPropertyRules<T, TElement, TPropertyType> Must(Func<bool> func, string errorMessage);
        Task<IPropertyRules<T, TElement, TPropertyType>> MustAsync(Func<Task<bool>> func) => MustAsync(func, "is invalid.");
        Task<IPropertyRules<T, TElement, TPropertyType>> MustAsync(Func<Task<bool>> func, string errorMessage);
    }
}