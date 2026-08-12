using Phauthentic.Specification.Examples.ECommerce.Specifications.Product;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class PriceRangeTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenPriceWithinRange()
    {
        var product = TestFactory.CreateProduct(price: 50m);

        var spec = new PriceRange(10m, 100m);

        Assert.True(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenPriceBelowRange()
    {
        var product = TestFactory.CreateProduct(price: 5m);

        var spec = new PriceRange(10m, 100m);

        Assert.False(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenPriceAboveRange()
    {
        var product = TestFactory.CreateProduct(price: 150m);

        var spec = new PriceRange(10m, 100m);

        Assert.False(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenBoundsAreUnboundedAndPriceIsAnyValue()
    {
        var product = TestFactory.CreateProduct(price: 999999m);

        var spec = new PriceRange();

        Assert.True(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenOnlyMinPriceSetAndPriceMeetsIt()
    {
        var product = TestFactory.CreateProduct(price: 50m);

        var spec = new PriceRange(minPrice: 10m);

        Assert.True(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOnlyMinPriceSetAndPriceBelowIt()
    {
        var product = TestFactory.CreateProduct(price: 5m);

        var spec = new PriceRange(minPrice: 10m);

        Assert.False(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenOnlyMaxPriceSetAndPriceMeetsIt()
    {
        var product = TestFactory.CreateProduct(price: 50m);

        var spec = new PriceRange(maxPrice: 100m);

        Assert.True(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOnlyMaxPriceSetAndPriceAboveIt()
    {
        var product = TestFactory.CreateProduct(price: 150m);

        var spec = new PriceRange(maxPrice: 100m);

        Assert.False(spec.IsSatisfiedBy(product));
    }
}
