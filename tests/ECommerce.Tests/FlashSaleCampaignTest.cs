using Phauthentic.Specification.Examples.ECommerce.Specifications.Campaigns;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class FlashSaleCampaignTest
{
    private static readonly DateTimeOffset WithinFlashSaleHours = new(2024, 1, 10, 13, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset OutsideFlashSaleHours = new(2024, 1, 10, 20, 0, 0, TimeSpan.Zero);

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAllConditionsAreMet()
    {
        var customer = TestFactory.CreateCustomer(lastFlashSaleUsed: null);
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "electronics")],
            createdAt: WithinFlashSaleHours);

        var spec = new FlashSaleCampaign();

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenCustomerUsedFlashSaleRecently()
    {
        var customer = TestFactory.CreateCustomer(lastFlashSaleUsed: DateTimeOffset.UtcNow.AddDays(-1));
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "electronics")],
            createdAt: WithinFlashSaleHours);

        var spec = new FlashSaleCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOrderDoesNotContainEligibleCategory()
    {
        var customer = TestFactory.CreateCustomer(lastFlashSaleUsed: null);
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "home")],
            createdAt: WithinFlashSaleHours);

        var spec = new FlashSaleCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenOutsideFlashSaleHours()
    {
        var customer = TestFactory.CreateCustomer(lastFlashSaleUsed: null);
        var order = TestFactory.CreateOrder(
            customer: customer,
            items: [TestFactory.CreateProduct(category: "electronics")],
            createdAt: OutsideFlashSaleHours);

        var spec = new FlashSaleCampaign();

        Assert.False(spec.IsSatisfiedBy(order));
    }
}
