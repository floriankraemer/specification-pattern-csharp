using Phauthentic.Specification.Examples.ECommerce.Specifications.Order;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class MinimumOrderValueTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenOrderMeetsMinimumValue()
    {
        var order = TestFactory.CreateOrder(totalAmount: 100m);

        var spec = new MinimumOrderValue(100m);

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOrderBelowMinimumValue()
    {
        var order = TestFactory.CreateOrder(totalAmount: 50m);

        var spec = new MinimumOrderValue(100m);

        Assert.False(spec.IsSatisfiedBy(order));
    }
}
