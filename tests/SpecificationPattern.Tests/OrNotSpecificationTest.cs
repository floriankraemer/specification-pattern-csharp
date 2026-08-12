using Xunit;

namespace Phauthentic.Specification.Tests;

public sealed class OrNotSpecificationTest
{
    [Fact]
    public void IsSatisfiedBy_RequiresLeftOrNotRight()
    {
        var spec1 = new ClosureSpecification<string>(candidate => candidate == "test");
        var spec2 = new ClosureSpecification<string>(candidate => candidate == "test2");

        var spec = new OrNotSpecification<string>(spec1, spec2);

        Assert.True(spec.IsSatisfiedBy("test"));
        Assert.False(spec.IsSatisfiedBy("test2"));
    }
}
