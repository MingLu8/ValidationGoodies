using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace ValidationGoodies.Tests
{
    public class Item
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }
    public class Order
    {
        public IEnumerable<Item> Items { get; set; }
    }
    public class ForPropertyTests : AbstractValidator<Order>
    {
        [Fact]
        public async Task Cascade_validates_all_rules()
        {
            RuleForEach(a => a.Items)
                .ForProperty(a => a.Name).UseRules(builder =>
                {
                    builder.Cascade().NotEmpty().Length(1, 10).Must(() => { return false;}, "rule must failed, third errors.");
                });
           var order = new Order{Items = new []{ new Item()}};
           
           var result = await this.ValidateAsync(order);

           result.Errors.Should().NotBeEmpty();
            result.Errors.Count.Should().Be(3);
           result.Errors[0].ErrorMessage.Should().Be("'Items[0].Name' must not be empty.");
           result.Errors[1].ErrorMessage.Should().Be("'Items[0].Name' must be between 1 and 10 characters. You entered 0 characters.");
           result.Errors[2].ErrorMessage.Should().Be("'Items[0].Name' rule must failed, third errors.");
        }

        [Fact]
        public async Task NoCascade_validation_stops_when_a_rule_fails()
        {
            RuleForEach(a => a.Items)
                .ForProperty(a => a.Name).UseRules(builder =>
                {
                    builder.NotEmpty().Length(1, 10).Must(() => { return false; }, "rule must failed, third errors.");
                });
            var order = new Order { Items = new[] { new Item() } };

            var result = await this.ValidateAsync(order);

            result.Errors.Should().NotBeEmpty();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should().Be("'Items[0].Name' must not be empty.");
        }

        [Fact]
        public async Task Cascade_validates_with_more_than_one_items()
        {
            RuleForEach(a => a.Items)
                .ForProperty(a => a.Name).UseRules(builder =>
                {
                    builder.Cascade().NotEmpty().Length(1, 10).Must(() => { return false; }, "rule must failed, third errors.");
                });
            var order = new Order { Items = new[] { new Item(), new Item() } };

            var result = await this.ValidateAsync(order);

            result.Errors.Should().NotBeEmpty();
            result.Errors.Count.Should().Be(6);
            result.Errors[0].ErrorMessage.Should().Be("'Items[0].Name' must not be empty.");
            result.Errors[1].ErrorMessage.Should().Be("'Items[0].Name' must be between 1 and 10 characters. You entered 0 characters.");
            result.Errors[2].ErrorMessage.Should().Be("'Items[0].Name' rule must failed, third errors.");
            result.Errors[3].ErrorMessage.Should().Be("'Items[1].Name' must not be empty.");
            result.Errors[4].ErrorMessage.Should().Be("'Items[1].Name' must be between 1 and 10 characters. You entered 0 characters.");
            result.Errors[5].ErrorMessage.Should().Be("'Items[1].Name' rule must failed, third errors.");
        }

        [Fact]
        public async Task NoCascade_validates_with_more_than_one_items()
        {
            RuleForEach(a => a.Items)
                .ForProperty(a => a.Name).UseRules(builder =>
                {
                    builder.NotEmpty().Length(1, 10).Must(() => { return false; }, "rule must failed, third errors.");
                });
            var order = new Order { Items = new[] { new Item(), new Item() } };

            var result = await this.ValidateAsync(order);

            result.Errors.Should().NotBeEmpty();
            result.Errors.Count.Should().Be(2);
            result.Errors[0].ErrorMessage.Should().Be("'Items[0].Name' must not be empty.");
            result.Errors[1].ErrorMessage.Should().Be("'Items[1].Name' must not be empty.");
        }

        [Fact]
        public async Task MustAsyncTest()
        {
            RuleForEach(a => a.Items)
                .ForProperty(a => a.Name).UseRulesAsync((builder, t) => builder.Cascade().NotEmpty().Length(1, 10).MustAsync(CheckAsync, "rule must failed, third errors."));
            var order = new Order { Items = new[] { new Item() } };

            var result = await this.ValidateAsync(order);

            result.Errors.Should().NotBeEmpty();
            result.Errors.Count.Should().Be(3);
            result.Errors[0].ErrorMessage.Should().Be("'Items[0].Name' must not be empty.");
            result.Errors[1].ErrorMessage.Should().Be("'Items[0].Name' must be between 1 and 10 characters. You entered 0 characters.");
            result.Errors[2].ErrorMessage.Should().Be("'Items[0].Name' rule must failed, third errors.");
        }

        [Fact]
        public async Task MustAsync_without_cancellation_token()
        {
            RuleForEach(a => a.Items)
                .ForProperty(a => a.Name).UseRulesAsync(builder => builder.Cascade().NotEmpty().Length(1, 10).MustAsync(CheckAsync, "rule must failed, third errors."));
            var order = new Order { Items = new[] { new Item() } };

            var result = await this.ValidateAsync(order);

            result.Errors.Should().NotBeEmpty();
            result.Errors.Count.Should().Be(3);
            result.Errors[0].ErrorMessage.Should().Be("'Items[0].Name' must not be empty.");
            result.Errors[1].ErrorMessage.Should().Be("'Items[0].Name' must be between 1 and 10 characters. You entered 0 characters.");
            result.Errors[2].ErrorMessage.Should().Be("'Items[0].Name' rule must failed, third errors.");
        }

        [Theory]
        [InlineData(5, 10, true)]
        [InlineData(0, 5, true)]
        [InlineData(5, 0, false)]
        [InlineData(0, 0, true)]
        [InlineData(-10, -5, true)]
        public async Task Max_test(int value, int max, bool isValid)
        {
            RuleForEach(a => a.Items)
                .ForProperty(a => a.Price).UseRules(builder =>
                {
                    builder.Max(max);
                });
            var order = new Order { Items = new[] { new Item{Price = value} } };

            var result = await this.ValidateAsync(order);
            result.IsValid.Should().Be(isValid);
            if(!isValid)
                result.Errors[0].ErrorMessage.Should().Be($"'Items[0].Price' cannot be greater than {max}, You entered {value}.");
        }

        [Theory]
        [InlineData(5, 10, false)]
        [InlineData(0, 5, false)]
        [InlineData(5, 0, true)]
        [InlineData(0, 0, true)]
        [InlineData(-10, -5, false)]
        public async Task Min_test(int value, int min, bool isValid)
        {
            RuleForEach(a => a.Items)
                .ForProperty(a => a.Price).UseRules(builder =>
                {
                    builder.Min(min);
                });
            var order = new Order { Items = new[] { new Item { Price = value } } };

            var result = await this.ValidateAsync(order);
            result.IsValid.Should().Be(isValid);
            if (!isValid)
                result.Errors[0].ErrorMessage.Should().Be($"'Items[0].Price' cannot be less than {min}, You entered {value}.");
        }

        [Theory]
        [InlineData("xx", 5, 10, false)]
        [InlineData("x", 0, 5, true)]
        public async Task Length_test(string value, int min, int max, bool isValid)
        {
            RuleForEach(a => a.Items)
                .ForProperty(a => a.Name).UseRules(builder =>
                {
                    builder.Length(min, max);
                });
            var order = new Order { Items = new[] { new Item { Name = value } } };

            var result = await this.ValidateAsync(order);
            result.IsValid.Should().Be(isValid);
            if (!isValid)
                result.Errors[0].ErrorMessage.Should().Be($"'Items[0].Name' must be between {min} and {max} characters. You entered {value.Length} characters.");
        }

        private Task<bool> CheckAsync()
        {
            return Task.FromResult(false);
        }
    }
}
