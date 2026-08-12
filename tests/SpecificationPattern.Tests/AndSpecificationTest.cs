using Xunit;

namespace Phauthentic.Specification.Tests;

public sealed class AndSpecificationTest
{
    [Fact]
    public void IsSatisfiedBy_RequiresBothSpecifications()
    {
        var spec1 = new ClosureSpecification<string>(candidate => candidate == "test");
        var spec2 = new ClosureSpecification<string>(candidate => candidate is not null);

        var spec = new AndSpecification<string>(spec1, spec2);

        Assert.True(spec.IsSatisfiedBy("test"));
        Assert.False(spec.IsSatisfiedBy("test2"));
    }
}
