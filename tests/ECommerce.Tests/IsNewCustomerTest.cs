using Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class IsNewCustomerTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAccountWithinMaxAge()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-5));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new IsNewCustomer(30);

        Assert.True(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalseWhenAccountOlderThanMaxAge()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-100));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new IsNewCustomer(30);

        Assert.False(spec.IsSatisfiedBy(order));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrueWhenAccountAgeEqualsMaxAge()
    {
        var customer = TestFactory.CreateCustomer(accountCreatedAt: DateTimeOffset.UtcNow.AddDays(-30));
        var order = TestFactory.CreateOrder(customer: customer);

        var spec = new IsNewCustomer(30);

        Assert.True(spec.IsSatisfiedBy(order));
    }
}
