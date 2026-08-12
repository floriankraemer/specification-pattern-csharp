using Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class AccountAgeTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAccountAgeMeetsMinimumThreshold()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-200));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new AccountAge(180, AccountAgeComparison.Minimum);

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenAccountAgeBelowMinimumThreshold()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-10));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new AccountAge(180, AccountAgeComparison.Minimum);

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAccountAgeEqualsMinimumThreshold()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-180));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new AccountAge(180, AccountAgeComparison.Minimum);

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAccountAgeWithinMaximumThreshold()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-10));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new AccountAge(30, AccountAgeComparison.Maximum);

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenAccountAgeExceedsMaximumThreshold()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-100));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new AccountAge(30, AccountAgeComparison.Maximum);

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAccountAgeEqualsMaximumThreshold()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-30));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new AccountAge(30, AccountAgeComparison.Maximum);

        Assert.True(spec.IsSatisfiedBy(order));
    }
}
