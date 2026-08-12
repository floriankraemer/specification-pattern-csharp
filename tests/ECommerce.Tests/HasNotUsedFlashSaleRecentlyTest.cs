using Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class HasNotUsedFlashSaleRecentlyTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenNeverUsedFlashSale()
    {
        var customer = TestFactory.CreateCustomer(lastFlashSaleUsed: null);
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new HasNotUsedFlashSaleRecently(7);

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenLastUsedOutsideCooldown()
    {
        var customer = TestFactory.CreateCustomer(lastFlashSaleUsed: DateTimeOffset.UtcNow.AddDays(-10));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new HasNotUsedFlashSaleRecently(7);

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenLastUsedWithinCooldown()
    {
        var customer = TestFactory.CreateCustomer(lastFlashSaleUsed: DateTimeOffset.UtcNow.AddDays(-1));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new HasNotUsedFlashSaleRecently(7);

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenLastUsedEqualsCooldown()
    {
        var customer = TestFactory.CreateCustomer(lastFlashSaleUsed: DateTimeOffset.UtcNow.AddDays(-7));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new HasNotUsedFlashSaleRecently(7);

        Assert.True(spec.IsSatisfiedBy(order));
    }
}
