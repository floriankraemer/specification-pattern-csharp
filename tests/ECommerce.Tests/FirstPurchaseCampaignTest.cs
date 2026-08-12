using Phauthentic.Specification.Examples.ECommerce.Specifications.Campaigns;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class FirstPurchaseCampaignTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAllConditionsAreMet()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-5));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isDigital: false, name: "Backpack", category: "fashion")],
            totalAmount: 60m,
            isFirstOrder: true);

        var spec = new FirstPurchaseCampaign();

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenCustomerIsNotNew()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-100));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isDigital: false, name: "Backpack", category: "fashion")],
            totalAmount: 60m,
            isFirstOrder: true);

        var spec = new FirstPurchaseCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenNotFirstOrder()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-5));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isDigital: false, name: "Backpack", category: "fashion")],
            totalAmount: 60m,
            isFirstOrder: false);

        var spec = new FirstPurchaseCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOrderValueBelowMinimum()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-5));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isDigital: false, name: "Backpack", category: "fashion")],
            totalAmount: 30m,
            isFirstOrder: true);

        var spec = new FirstPurchaseCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOrderContainsDigitalItems()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-5));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isDigital: true, name: "E-Book", category: "books")],
            totalAmount: 60m,
            isFirstOrder: true);

        var spec = new FirstPurchaseCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOrderContainsGiftCards()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-5));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(isDigital: false, name: "Gift Card $50", category: "gift-cards")],
            totalAmount: 60m,
            isFirstOrder: true);

        var spec = new FirstPurchaseCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }
}
