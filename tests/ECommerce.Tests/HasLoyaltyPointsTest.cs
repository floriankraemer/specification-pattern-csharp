using Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class HasLoyaltyPointsTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenPointsMeetMinimum()
    {
        var customer = TestFactory.CreateCustomer(loyaltyPoints: 1000);
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new HasLoyaltyPoints(1000);

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenPointsBelowMinimum()
    {
        var customer = TestFactory.CreateCustomer(loyaltyPoints: 500);
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new HasLoyaltyPoints(1000);

        Assert.False(spec.IsSatisfiedBy(order));
    }
}
