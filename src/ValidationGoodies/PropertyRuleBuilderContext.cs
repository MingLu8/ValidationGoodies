using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace ValidationGoodies
{
    public class PropertyRuleBuilderContext<T, TElement, TPropertyType> : IPropertyRuleBuilderContext<T, TElement, TPropertyType> where TPropertyType : IComparable
    {
        public bool NoCascade { get; private set; } = true;
        public bool Failed { get; private set; }
        public string PropertyName { get; }
        public TPropertyType PropertyValue { get; }
        public TElement InstanceToValidate { get; }
        public T ParentInstanceToValidate => Context.InstanceToValidate;
        public ValidationContext<T> Context { get; }

        public PropertyRuleBuilderContext(string propertyName, TPropertyType propertyValue, TElement instanceToValidate, ValidationContext<T> context)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            InstanceToValidate = instanceToValidate;
            Context = context;
        }

        public virtual PropertyRuleBuilderContext<T, TElement, TPropertyType> Cascade()
        {
            NoCascade = false;
            return this;
        }

        public virtual PropertyRuleBuilderContext<T, TElement, TPropertyType> NotEmpty()
        {
            if (NoCascade && Failed || PropertyValue != null) return this;

            return AddFailure("must not be empty.");
        }

        public virtual PropertyRuleBuilderContext<T, TElement, TPropertyType> Max(TPropertyType max)
        {
            if (NoCascade && Failed) return this;
            return PropertyValue?.CompareTo(max) <= 0 ? this : AddFailure($"cannot be greater than {max}, You entered {PropertyValue}.");
        }

        public virtual PropertyRuleBuilderContext<T, TElement, TPropertyType> Min(TPropertyType min)
        {
            if (NoCascade && Failed) return this;

            return PropertyValue?.CompareTo(min) >= 0 ? this : AddFailure($"cannot be less than {min}, You entered {PropertyValue}.");
        }

        public virtual PropertyRuleBuilderContext<T, TElement, TPropertyType> Length(int min, int max)
        {
            if (NoCascade && Failed) return this;
            var length = PropertyValue?.ToString().Length ?? 0;
            if (length <= max && length >= min) return this;

            return AddFailure($"must be between {min} and {max} characters. You entered {length} characters.");
        }
      
        public virtual PropertyRuleBuilderContext<T, TElement, TPropertyType> Must(Func<bool> func, string errorMessage)
        {
            if (NoCascade && Failed || func()) return this;

            return AddFailure(errorMessage);
        }

        public virtual async Task<PropertyRuleBuilderContext<T, TElement, TPropertyType>> MustAsync(Func<Task<bool>> func, string errorMessage)
        {
            if (NoCascade && Failed || await func()) return this;

            return AddFailure(errorMessage);
        }

        protected virtual PropertyRuleBuilderContext<T, TElement, TPropertyType> AddFailure(string errorMessage)
        {
            Failed = true;
            Context.AddFailure(PropertyName, $"'{Context.PropertyName}.{PropertyName}' {errorMessage}");
            return this;
        }
    }
}