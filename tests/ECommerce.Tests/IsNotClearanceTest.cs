using Phauthentic.Specification.Examples.ECommerce.Specifications.Product;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class IsNotClearanceTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenProductIsNotClearance()
    {
        var product = TestFactory.CreateProduct(isClearance: false);

        var spec = new IsNotClearance();

        Assert.True(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenProductIsClearance()
    {
        var product = TestFactory.CreateProduct(isClearance: true);

        var spec = new IsNotClearance();

        Assert.False(spec.IsSatisfiedBy(product));
    }
}
