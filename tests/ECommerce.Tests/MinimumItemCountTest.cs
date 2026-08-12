using Phauthentic.Specification.Examples.ECommerce.Specifications.Order;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class MinimumItemCountTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenItemCountMeetsMinimum()
    {
        var order = TestFactory.CreateOrder(itemCount: 3);

        var spec = new MinimumItemCount(3);

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenItemCountBelowMinimum()
    {
        var order = TestFactory.CreateOrder(itemCount: 2);

        var spec = new MinimumItemCount(3);

        Assert.False(spec.IsSatisfiedBy(order));
    }
}
