using System;
using FluentValidation;

namespace ValidationGoodies
{
    public class ChildPropertyRuleBuilder<T, TElement>
    {
        public bool NoCascade { get; private set; } = true;
        public bool Failed { get; private set; }
        public string PropertyName { get; }
        public object PropertyValue { get; }
        public TElement InstanceToValidate { get; }
        public T ParentInstanceToValidate => Context.InstanceToValidate;
        public ValidationContext<T> Context { get; }

        public ChildPropertyRuleBuilder(string propertyName, object propertyValue, TElement instanceToValidate, ValidationContext<T> context)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            InstanceToValidate = instanceToValidate;
            Context = context;
        }

        public virtual ChildPropertyRuleBuilder<T, TElement> Cascade()
        {
            NoCascade = false;
            return this;
        }

        public virtual ChildPropertyRuleBuilder<T, TElement> NotEmpty()
        {
            if (NoCascade && Failed || PropertyValue != null) return this;

            return AddFailure("must not be empty.");
        }

        public virtual ChildPropertyRuleBuilder<T, TElement> Length(int min, int max)
        {
            if (NoCascade && Failed) return this;
            var length = PropertyValue?.ToString().Length ?? 0;
            if (length < max && length > min) return this;

            return AddFailure($"must be between {min} and {max} characters. You entered {length} characters.");
        }
        public virtual ChildPropertyRuleBuilder<T, TElement> Must(Func<TElement, bool> func, string errorMessage)
        {
            if (NoCascade && Failed || func(InstanceToValidate)) return this;

            return AddFailure(errorMessage);
        }
        public virtual ChildPropertyRuleBuilder<T, TElement> Must(Func<T, TElement, bool> func, string errorMessage)
        {
            if (NoCascade && Failed || func(ParentInstanceToValidate, InstanceToValidate)) return this;

            return AddFailure(errorMessage);
        }

        public virtual ChildPropertyRuleBuilder<T, TElement> Must(Func<T, TElement, ValidationContext<T>, bool> func, string errorMessage)
        {
            if (NoCascade && Failed || func(ParentInstanceToValidate, InstanceToValidate, Context)) return this;

            return AddFailure(errorMessage);
        }

        public virtual ChildPropertyRuleBuilder<T, TElement> Must(Func<string, object, T, TElement, ValidationContext<T>, bool> func, string errorMessage)
        {
            if (NoCascade && Failed || func(PropertyName, PropertyValue, ParentInstanceToValidate, InstanceToValidate, Context)) return this;

            return AddFailure(errorMessage);
        }

        protected virtual ChildPropertyRuleBuilder<T, TElement> AddFailure(string errorMessage)
        {
            Failed = true;
            Context.AddFailure(PropertyName, $"'{Context.PropertyName}.{PropertyName}' {errorMessage}");
            return this;
        }
    }
}