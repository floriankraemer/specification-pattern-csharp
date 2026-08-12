using Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class IsVipCustomerTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenCustomerTierIsVip()
    {
        var customer = TestFactory.CreateCustomer(tier: "gold");
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new IsVipCustomer();

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenCustomerTierIsNotVip()
    {
        var customer = TestFactory.CreateCustomer(tier: "bronze");
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new IsVipCustomer();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenCustomerTierIsInCustomTierList()
    {
        var customer = TestFactory.CreateCustomer(tier: "silver");
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new IsVipCustomer(["silver", "diamond"]);

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenCustomerTierIsNotInCustomTierListEvenIfDefaultVip()
    {
        var customer = TestFactory.CreateCustomer(tier: "gold");
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new IsVipCustomer(["silver", "diamond"]);

        Assert.False(spec.IsSatisfiedBy(order));
    }
}
