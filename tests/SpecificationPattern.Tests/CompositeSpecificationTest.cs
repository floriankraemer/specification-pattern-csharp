using Xunit;

namespace Phauthentic.Specification.Tests;

public sealed class CompositeSpecificationTest
{
    [Fact]
    public void Constructor_AcceptsValidSpecifications()
    {
        var spec1 = new ClosureSpecification<object>(candidate => Equals(candidate, "test"));
        var spec2 = new ClosureSpecification<object>(candidate => candidate is string);

        var composite = new CompositeSpecification<object>([spec1, spec2]);

        Assert.IsType<CompositeSpecification<object>>(composite);
    }

    [Fact]
    public void Constructor_ThrowsForNullSpecification()
    {
        // Unlike the PHP original (a runtime-checked array of mixed values), the C# constructor
        // is generically typed, so the compiler already rejects non-ISpecification<T> elements.
        // The one remaining invalid input representable at runtime is a null element.
        var spec1 = new ClosureSpecification<object>(candidate => Equals(candidate, "test"));

        var exception = Assert.Throws<ArgumentException>(
            () => new CompositeSpecification<object>([spec1, null!]));

        Assert.Contains("is not an instance of", exception.Message);
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAllSatisfied()
    {
        var spec1 = new ClosureSpecification<object>(candidate => Equals(candidate, "test"));
        var spec2 = new ClosureSpecification<object>(candidate => candidate is string);

        var composite = new CompositeSpecification<object>([spec1, spec2]);

        Assert.True(composite.IsSatisfiedBy("test"));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenAnyNotSatisfied()
    {
        var spec1 = new ClosureSpecification<object>(candidate => Equals(candidate, "test"));
        var spec2 = new ClosureSpecification<object>(candidate => candidate is string);

        var composite = new CompositeSpecification<object>([spec1, spec2]);

        Assert.False(composite.IsSatisfiedBy("test2"));
        Assert.False(composite.IsSatisfiedBy(123));
    }

    [Fact]
    public void IsSatisfiedBy_WithEmptyList_ReturnsTrue()
    {
        var composite = new CompositeSpecification<object>([]);

        // Empty composite should return true (all zero specifications are satisfied).
        Assert.True(composite.IsSatisfiedBy("anything"));
    }

    [Fact]
    public void IsSatisfiedBy_WithSingleSpecification()
    {
        var spec1 = new ClosureSpecification<object>(candidate => Equals(candidate, "test"));

        var composite = new CompositeSpecification<object>([spec1]);

        Assert.True(composite.IsSatisfiedBy("test"));
        Assert.False(composite.IsSatisfiedBy("test2"));
    }

    [Fact]
    public void IsSatisfiedBy_WithMultipleSpecifications_FirstFails()
    {
        var spec1 = new ClosureSpecification<object>(candidate => Equals(candidate, "test"));
        var spec2 = new ClosureSpecification<object>(candidate => candidate is string);
        var spec3 = new ClosureSpecification<object>(candidate => ((string)candidate).Length > 3);

        var composite = new CompositeSpecification<object>([spec1, spec2, spec3]);

        Assert.False(composite.IsSatisfiedBy("test2"));
    }

    [Fact]
    public void IsSatisfiedBy_WithMultipleSpecifications_MiddleFails()
    {
        var spec1 = new ClosureSpecification<object>(candidate => Equals(candidate, "test"));
        var spec2 = new ClosureSpecification<object>(candidate => candidate is int);
        var spec3 = new ClosureSpecification<object>(candidate => candidate.ToString()!.Length > 3);

        var composite = new CompositeSpecification<object>([spec1, spec2, spec3]);

        Assert.False(composite.IsSatisfiedBy("test"));
    }
}
