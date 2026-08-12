using Xunit;

namespace Phauthentic.Specification.Tests;

public sealed class OrSpecificationTest
{
    [Fact]
    public void IsSatisfiedBy_RequiresEitherSpecification()
    {
        var is2000 = new ClosureSpecification<string>(candidate => candidate == "2000");
        var is2022 = new ClosureSpecification<string>(candidate => candidate == "2022");

        var spec = new OrSpecification<string>(is2000, is2022);

        Assert.True(spec.IsSatisfiedBy("2000"));
        Assert.True(spec.IsSatisfiedBy("2022"));
        Assert.False(spec.IsSatisfiedBy("2030"));
    }
}
