using Phauthentic.Specification.Examples.ECommerce.Specifications.Order;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class ContainsProductCategoryTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenOrderContainsRequiredCategory()
    {
        var order = TestFactory.CreateOrder(items: [TestFactory.CreateProduct(category: "electronics")]);

        var spec = new ContainsProductCategory("electronics");

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOrderDoesNotContainRequiredCategory()
    {
        var order = TestFactory.CreateOrder(items: [TestFactory.CreateProduct(category: "books")]);

        var spec = new ContainsProductCategory("electronics");

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenOrderMatchesSecondCategoryInMultiCategoryList()
    {
        var order = TestFactory.CreateOrder(items: [TestFactory.CreateProduct(category: "fashion")]);

        var spec = new ContainsProductCategory(["electronics", "fashion"]);

        Assert.True(spec.IsSatisfiedBy(order));
    }
}
