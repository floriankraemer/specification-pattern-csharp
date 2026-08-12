using Phauthentic.Specification.Examples.ECommerce.Specifications.Product;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class ProductCategoryTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenCategoryIsAllowed()
    {
        var product = TestFactory.CreateProduct(category: "electronics");

        var spec = new ProductCategory(["electronics", "fashion"]);

        Assert.True(spec.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenCategoryIsNotAllowed()
    {
        var product = TestFactory.CreateProduct(category: "books");

        var spec = new ProductCategory(["electronics", "fashion"]);

        Assert.False(spec.IsSatisfiedBy(product));
    }
}
