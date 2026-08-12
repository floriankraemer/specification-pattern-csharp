using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;
using Xunit;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Tests;

public sealed class IsNotDisputedTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenItemIsNotDisputed()
    {
        var asOf = new DateOnly(2026, 1, 31);
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), asOf.AddDays(-30), "test", dueDate: asOf));
        var item = account.OpenItems.Single();

        var spec = new IsNotDisputed();

        Assert.True(spec.IsSatisfiedBy(item));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsDisputed()
    {
        var asOf = new DateOnly(2026, 1, 31);
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), asOf.AddDays(-30), "test", dueDate: asOf));
        account.DisputeOpenItem("INV-1", true);
        var item = account.OpenItems.Single();

        var spec = new IsNotDisputed();

        Assert.False(spec.IsSatisfiedBy(item));
    }
}
