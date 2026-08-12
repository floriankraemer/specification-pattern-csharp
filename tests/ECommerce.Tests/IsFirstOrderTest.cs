using Phauthentic.Specification.Examples.ECommerce.Specifications.Order;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class IsFirstOrderTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenOrderIsFirstOrder()
    {
        var order = TestFactory.CreateOrder(isFirstOrder: true);

        var spec = new IsFirstOrder();

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOrderIsNotFirstOrder()
    {
        var order = TestFactory.CreateOrder(isFirstOrder: false);

        var spec = new IsFirstOrder();

        Assert.False(spec.IsSatisfiedBy(order));
    }
}
