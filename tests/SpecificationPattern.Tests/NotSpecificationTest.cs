using Xunit;

namespace Phauthentic.Specification.Tests;

public sealed class NotSpecificationTest
{
    [Fact]
    public void IsSatisfiedBy_InvertsWrappedSpecification()
    {
        var spec1 = new ClosureSpecification<string>(candidate => candidate == "test");

        var spec = new NotSpecification<string>(spec1);

        Assert.False(spec.IsSatisfiedBy("test"));
        Assert.True(spec.IsSatisfiedBy("test1"));
    }
}
