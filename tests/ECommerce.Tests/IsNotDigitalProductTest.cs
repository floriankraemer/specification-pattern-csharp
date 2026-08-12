using Phauthentic.Specification.Examples.ECommerce.Specifications.Product;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class IsNotDigitalProductTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenProductIsPhysicalAndNotGiftCard()
    {
        var product = TestFactory.CreateProduct(isDigital: false, name: "Wireless Mouse", category: "electronics");

        var spec = new IsNotDigitalProduct();

        Assert.True(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenProductIsDigital()
    {
        var product = TestFactory.CreateProduct(isDigital: true);

        var spec = new IsNotDigitalProduct();

        Assert.False(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenProductIsGiftCard()
    {
        var product = TestFactory.CreateProduct(isDigital: false, name: "Gift Card $50", category: "gift-cards");

        var spec = new IsNotDigitalProduct();

        Assert.False(spec.IsSatisfiedBy(product));
    }
}
