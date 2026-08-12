using Phauthentic.Specification.Examples.ECommerce.Specifications.Campaigns;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class BlackFridayVipCampaignTest
{
    private static readonly DateTimeOffset WithinCampaignWindow = new(2025, 11, 25, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset OutsideCampaignWindow = new(2025, 12, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAllConditionsAreMet()
    {
        var customer = TestFactory.CreateCustomer(tier: "gold", accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-200));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "electronics")],
            totalAmount: 150m,
            createdAt: WithinCampaignWindow);

        var spec = new BlackFridayVipCampaign();

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenCustomerIsNotVip()
    {
        var customer = TestFactory.CreateCustomer(tier: "bronze", accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-200));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "electronics")],
            totalAmount: 150m,
            createdAt: WithinCampaignWindow);

        var spec = new BlackFridayVipCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOrderValueBelowMinimum()
    {
        var customer = TestFactory.CreateCustomer(tier: "gold", accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-200));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "electronics")],
            totalAmount: 50m,
            createdAt: WithinCampaignWindow);

        var spec = new BlackFridayVipCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenOrderContainsFashionCategory()
    {
        var customer = TestFactory.CreateCustomer(tier: "gold", accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-200));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "fashion")],
            totalAmount: 150m,
            createdAt: WithinCampaignWindow);

        var spec = new BlackFridayVipCampaign();

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOrderDoesNotContainEligibleCategory()
    {
        var customer = TestFactory.CreateCustomer(tier: "gold", accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-200));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "books")],
            totalAmount: 150m,
            createdAt: WithinCampaignWindow);

        var spec = new BlackFridayVipCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenAccountIsTooNew()
    {
        var customer = TestFactory.CreateCustomer(tier: "gold", accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-10));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "electronics")],
            totalAmount: 150m,
            createdAt: WithinCampaignWindow);

        var spec = new BlackFridayVipCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOutsideCampaignDateRange()
    {
        var customer = TestFactory.CreateCustomer(tier: "gold", accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-200));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "electronics")],
            totalAmount: 150m,
            createdAt: OutsideCampaignWindow);

        var spec = new BlackFridayVipCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }
}
