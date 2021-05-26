using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace ValidationGoodies
{
    public class ChildPropertyRuleBuilder<T, TElement, PropertyType>
    {
        public bool NoCascade { get; private set; } = true;
        public bool Failed { get; private set; }
        public string PropertyName { get; }
        public PropertyType PropertyValue { get; }
        public TElement InstanceToValidate { get; }
        public T ParentInstanceToValidate => Context.InstanceToValidate;
        public ValidationContext<T> Context { get; }

        public ChildPropertyRuleBuilder(string propertyName, PropertyType propertyValue, TElement instanceToValidate, ValidationContext<T> context)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            InstanceToValidate = instanceToValidate;
            Context = context;
        }

        public virtual ChildPropertyRuleBuilder<T, TElement, PropertyType> Cascade()
        {
            NoCascade = false;
            return this;
        }

        public virtual ChildPropertyRuleBuilder<T, TElement, PropertyType> NotEmpty()
        {
            if (NoCascade && Failed || PropertyValue != null) return this;

            return AddFailure("must not be empty.");
        }

        public virtual ChildPropertyRuleBuilder<T, TElement, PropertyType> Max(int max)
        {
            if (NoCascade && Failed) return this;
            dynamic v = PropertyValue;
            if (v <= max) return this;

            return AddFailure($"cannot be greater than {max}, You entered {v}.");
        }

        public virtual ChildPropertyRuleBuilder<T, TElement, PropertyType> Length(int min, int max)
        {
            if (NoCascade && Failed) return this;
            var length = PropertyValue?.ToString().Length ?? 0;
            if (length <= max && length >= min) return this;

            return AddFailure($"must be between {min} and {max} characters. You entered {length} characters.");
        }
      
        public virtual ChildPropertyRuleBuilder<T, TElement, PropertyType> Must(Func<bool> func, string errorMessage)
        {
            if (NoCascade && Failed || func()) return this;

            return AddFailure(errorMessage);
        }

        public virtual async Task<ChildPropertyRuleBuilder<T, TElement, PropertyType>> MustAsync(Func<Task<bool>> func, string errorMessage)
        {
            if (NoCascade && Failed || await func()) return this;

            return AddFailure(errorMessage);
        }

        protected virtual ChildPropertyRuleBuilder<T, TElement, PropertyType> AddFailure(string errorMessage)
        {
            Failed = true;
            Context.AddFailure(PropertyName, $"'{Context.PropertyName}.{PropertyName}' {errorMessage}");
            return this;
        }
    }
}