using Xunit;

namespace Phauthentic.Specification.Tests;

/// <summary>
/// Tests the combinator methods (And, AndNot, Or, OrNot, Not) provided by the abstract
/// <see cref="Specification{T}"/> base class.
/// </summary>
public sealed class SpecificationTest
{
    [Fact]
    public void And_ReturnsAndSpecification()
    {
        var spec1 = new ClosureSpecification<string>(candidate => candidate == "test");
        var spec2 = new ClosureSpecification<string>(candidate => candidate is not null);

        var result = spec1.And(spec2);

        Assert.IsType<AndSpecification<string>>(result);
        Assert.True(result.IsSatisfiedBy("test"));
        Assert.False(result.IsSatisfiedBy("test2"));
    }

    [Fact]
    public void AndNot_ReturnsAndNotSpecification()
    {
        var spec1 = new ClosureSpecification<string>(candidate => candidate == "test");
        var spec2 = new ClosureSpecification<string>(candidate => candidate == "test2");

        var result = spec1.AndNot(spec2);

        Assert.IsType<AndNotSpecification<string>>(result);
        Assert.True(result.IsSatisfiedBy("test"));
        Assert.False(result.IsSatisfiedBy("test2"));
    }

    [Fact]
    public void Or_ReturnsOrSpecification()
    {
        var spec1 = new ClosureSpecification<string>(candidate => candidate == "test");
        var spec2 = new ClosureSpecification<string>(candidate => candidate == "test2");

        var result = spec1.Or(spec2);

        Assert.IsType<OrSpecification<string>>(result);
        Assert.True(result.IsSatisfiedBy("test"));
        Assert.True(result.IsSatisfiedBy("test2"));
        Assert.False(result.IsSatisfiedBy("test3"));
    }

    [Fact]
    public void OrNot_ReturnsOrNotSpecification()
    {
        var spec1 = new ClosureSpecification<string>(candidate => candidate == "test");
        var spec2 = new ClosureSpecification<string>(candidate => candidate == "test2");

        var result = spec1.OrNot(spec2);

        Assert.IsType<OrNotSpecification<string>>(result);
        Assert.True(result.IsSatisfiedBy("test"));
        Assert.False(result.IsSatisfiedBy("test2"));
    }

    [Fact]
    public void Not_ReturnsNotSpecification()
    {
        var spec1 = new ClosureSpecification<string>(candidate => candidate == "test");

        var result = spec1.Not();

        Assert.IsType<NotSpecification<string>>(result);
        Assert.False(result.IsSatisfiedBy("test"));
        Assert.True(result.IsSatisfiedBy("test1"));
    }
}
