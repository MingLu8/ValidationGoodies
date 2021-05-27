using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;

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

        public virtual IPropertyRules<T, TElement, TPropertyType> Cascade()
        {
            NoCascade = false;
            return this;
        }

        public virtual IPropertyRules<T, TElement, TPropertyType> NotEmpty(string errorMessage)
        {
            if (NoCascade && Failed || NotEmptyInternal()) return this;

            return AddFailure(errorMessage);
        }

        public virtual IPropertyRules<T, TElement, TPropertyType> Max(TPropertyType max, string errorMessage)
        {
            if (NoCascade && Failed) return this;
            return PropertyValue?.CompareTo(max) <= 0 ? this : AddFailure(errorMessage);
        }

        public virtual IPropertyRules<T, TElement, TPropertyType> Min(TPropertyType min, string errorMessage)
        {
            if (NoCascade && Failed) return this;

            return PropertyValue?.CompareTo(min) >= 0 ? this : AddFailure(errorMessage);
        }

        public virtual IPropertyRules<T, TElement, TPropertyType> MaxLength(int max, string errorMessage)
        {
            if (NoCascade && Failed || MaxLengthInternal(max)) return this;

            return AddFailure(errorMessage);
        }

        public virtual IPropertyRules<T, TElement, TPropertyType> MinLength(int min, string errorMessage)
        {
            if (NoCascade && Failed || MinLengthInternal(min)) return this;

            return AddFailure(errorMessage);
        }

        public virtual IPropertyRules<T, TElement, TPropertyType> Length(int exactValue, string errorMessage)
        {
            if (NoCascade && Failed || LengthInternal(exactValue)) return this;
            
            return AddFailure(errorMessage);
        }

        public virtual IPropertyRules<T, TElement, TPropertyType> Length(int min, int max, string errorMessage)
        {
            if (NoCascade && Failed || LengthInternal(min, max)) return this;
            
            return AddFailure(errorMessage);
        }

        public virtual IPropertyRules<T, TElement, TPropertyType> Must(Func<bool> func, string errorMessage)
        {
            if (NoCascade && Failed || func()) return this;

            return AddFailure(errorMessage);
        }

        public virtual IPropertyRules<T, TElement, TPropertyType> Matches(string expression, string errorMessage)
        {
            if (NoCascade && Failed || MatchesInternal(expression)) return this;

            return AddFailure(errorMessage);
        }

        public virtual async Task<IPropertyRules<T, TElement, TPropertyType>> MustAsync(Func<Task<bool>> func, string errorMessage)
        {
            if (NoCascade && Failed || await func()) return this;

            return AddFailure(errorMessage);
        }

        protected bool NotEmptyInternal() => new NotEmptyValidator<T, TPropertyType>().IsValid(Context, PropertyValue);
        protected bool LengthInternal(int min, int max) => new LengthValidator<T>(min, max).IsValid(Context, PropertyValue?.ToString());
        protected bool LengthInternal(int exact) => new ExactLengthValidator<T>(exact).IsValid(Context, PropertyValue?.ToString());
        protected bool MaxLengthInternal(int max) => new MaximumLengthValidator<T>(max).IsValid(Context, PropertyValue?.ToString());
        protected bool MinLengthInternal(int min) => new MinimumLengthValidator<T>(min).IsValid(Context, PropertyValue?.ToString());
        protected bool MatchesInternal(string expression) => new RegularExpressionValidator<T>(expression).IsValid(Context, PropertyValue?.ToString());
        protected virtual IPropertyRules<T, TElement, TPropertyType> AddFailure(string errorMessage)
        {
            Failed = true;
            Context.AddFailure(PropertyName, $"'{Context.PropertyName}.{PropertyName}' {errorMessage}");
            return this;
        }
    }
}