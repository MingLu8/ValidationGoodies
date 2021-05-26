# ValidationGoodies
Contains helper code for object validation using fluent validations.

## ForProperty
Contains validation rules for child elment for collection property from the parent object.

``` C#
[Fact]
public void AddSectionTest()
{
    var config = Substitute.For<IConfiguration>().AddSection("a", "b");
    config.GetSection("a").Value.Should().Be("b");
}
```