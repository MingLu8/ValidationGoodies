using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace ValidationGoodies
{
    public class PropertyRules<T, TElement, TPropertyType> : IPropertyRules<T, TElement, TPropertyType> where TPropertyType : IComparable
    {
        public bool NoCascade { get; private set; } = true;
        public bool Failed { get; private set; }
        public string PropertyName { get; }
        public TPropertyType PropertyValue { get; }
        public TElement InstanceToValidate { get; }
        public T ParentInstanceToValidate => Context.InstanceToValidate;
        public ValidationContext<T> Context { get; }

        public PropertyRules(string propertyName, TPropertyType propertyValue, TElement instanceToValidate, ValidationContext<T> context)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            InstanceToValidate = instanceToValidate;
            Context = context;
        }

        public virtual PropertyRules<T, TElement, TPropertyType> Cascade()
        {
            NoCascade = false;
            return this;
        }

        public virtual PropertyRules<T, TElement, TPropertyType> NotEmpty()
        {
            if (NoCascade && Failed || PropertyValue != null) return this;

            return AddFailure("must not be empty.");
        }

        public virtual PropertyRules<T, TElement, TPropertyType> Max(TPropertyType max)
        {
            if (NoCascade && Failed) return this;
            return PropertyValue?.CompareTo(max) <= 0 ? this : AddFailure($"cannot be greater than {max}, You entered {PropertyValue}.");
        }

        public virtual PropertyRules<T, TElement, TPropertyType> Min(TPropertyType min)
        {
            if (NoCascade && Failed) return this;

            return PropertyValue?.CompareTo(min) >= 0 ? this : AddFailure($"cannot be less than {min}, You entered {PropertyValue}.");
        }

        public virtual PropertyRules<T, TElement, TPropertyType> Length(int exactValue)
        {
            if (NoCascade && Failed) return this;

            var length = PropertyValue?.ToString().Length;
            if (length != null && length == exactValue) return this;

            return AddFailure($"must be {exactValue} characters. You entered {length ?? 0} characters.");
        }

        public virtual PropertyRules<T, TElement, TPropertyType> Length(int min, int max)
        {
            if (NoCascade && Failed) return this;

            var length = PropertyValue?.ToString().Length;
            if (length != null && length <= max && length >= min) return this;

            return AddFailure($"must be between {min} and {max} characters. You entered {length ?? 0} characters.");
        }
      
        public virtual PropertyRules<T, TElement, TPropertyType> Must(Func<bool> func, string errorMessage)
        {
            if (NoCascade && Failed || func()) return this;

            return AddFailure(errorMessage);
        }

        public virtual async Task<PropertyRules<T, TElement, TPropertyType>> MustAsync(Func<Task<bool>> func, string errorMessage)
        {
            if (NoCascade && Failed || await func()) return this;

            return AddFailure(errorMessage);
        }

        protected virtual PropertyRules<T, TElement, TPropertyType> AddFailure(string errorMessage)
        {
            Failed = true;
            Context.AddFailure(PropertyName, $"'{Context.PropertyName}.{PropertyName}' {errorMessage}");
            return this;
        }
    }
}