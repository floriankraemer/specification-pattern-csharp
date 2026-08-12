using Phauthentic.Specification.Examples.ECommerce.Specifications.Campaigns;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class LoyaltyRewardCampaignTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAllConditionsAreMet()
    {
        var customer = TestFactory.CreateCustomer(loyaltyPoints: 1500, accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-60));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isClearance: false), TestFactory.CreateProduct(isClearance: false), TestFactory.CreateProduct(isClearance: false)],
            itemCount: 3);

        var spec = new LoyaltyRewardCampaign();

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenLoyaltyPointsBelowMinimum()
    {
        var customer = TestFactory.CreateCustomer(loyaltyPoints: 500, accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-60));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isClearance: false), TestFactory.CreateProduct(isClearance: false), TestFactory.CreateProduct(isClearance: false)],
            itemCount: 3);

        var spec = new LoyaltyRewardCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenCustomerIsTooNew()
    {
        var customer = TestFactory.CreateCustomer(loyaltyPoints: 1500, accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-5));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isClearance: false), TestFactory.CreateProduct(isClearance: false), TestFactory.CreateProduct(isClearance: false)],
            itemCount: 3);

        var spec = new LoyaltyRewardCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenItemCountBelowMinimum()
    {
        var customer = TestFactory.CreateCustomer(loyaltyPoints: 1500, accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-60));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isClearance: false), TestFactory.CreateProduct(isClearance: false)],
            itemCount: 2);

        var spec = new LoyaltyRewardCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOrderContainsClearanceItems()
    {
        var customer = TestFactory.CreateCustomer(loyaltyPoints: 1500, accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-60));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isClearance: true), TestFactory.CreateProduct(isClearance: false), TestFactory.CreateProduct(isClearance: false)],
            itemCount: 3);

        var spec = new LoyaltyRewardCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }
}
