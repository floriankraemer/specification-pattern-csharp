using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Account;
using Xunit;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Tests;

public sealed class RiskClassTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenAccountMatchesOneOfTheGivenClasses()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Preferred, "EUR");

        var spec = new RiskClass(CustomerRiskClass.Standard, CustomerRiskClass.Preferred);

        Assert.True(spec.IsSatisfiedBy(account));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAccountMatchesNoneOfTheGivenClasses()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.HighRisk, "EUR");

        var spec = new RiskClass(CustomerRiskClass.Standard, CustomerRiskClass.Preferred);

        Assert.False(spec.IsSatisfiedBy(account));
    }
}
