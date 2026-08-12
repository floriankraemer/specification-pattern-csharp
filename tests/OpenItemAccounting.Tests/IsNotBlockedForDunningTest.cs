using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Account;
using Xunit;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Tests;

public sealed class IsNotBlockedForDunningTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenAccountIsNotBlocked()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");

        var spec = new IsNotBlockedForDunning();

        Assert.True(spec.IsSatisfiedBy(account));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAccountIsBlocked()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Block();

        var spec = new IsNotBlockedForDunning();

        Assert.False(spec.IsSatisfiedBy(account));
    }
}
