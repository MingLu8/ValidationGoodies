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
                .ForProperty(a => a.Name, builder =>
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
                .ForProperty(a => a.Name, builder =>
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
                .ForProperty(a => a.Name, builder =>
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
                .ForProperty(a => a.Name, builder =>
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
                .ForPropertyAsync(a => a.Name, async (builder, t) =>
                { 
                    await builder.Cascade().NotEmpty().Length(1, 10).MustAsync(CheckAsync, "rule must failed, third errors.");
                });
            var order = new Order { Items = new[] { new Item() } };

            var result = await this.ValidateAsync(order);

            result.Errors.Should().NotBeEmpty();
            result.Errors.Count.Should().Be(3);
            result.Errors[0].ErrorMessage.Should().Be("'Items[0].Name' must not be empty.");
            result.Errors[1].ErrorMessage.Should().Be("'Items[0].Name' must be between 1 and 10 characters. You entered 0 characters.");
            result.Errors[2].ErrorMessage.Should().Be("'Items[0].Name' rule must failed, third errors.");
        }

        private Task<bool> CheckAsync()
        {
            return Task.FromResult(false);
        }
    }
}
