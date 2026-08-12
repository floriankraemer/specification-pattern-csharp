using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;
using Xunit;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Tests;

public sealed class IsOverdueTest
{
    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(1, 0, true)]
    [InlineData(5, 5, false)]
    [InlineData(6, 5, true)]
    public void IsSatisfiedBy_EvaluatesOverdueBeyondGraceDays(int daysPastDue, int graceDays, bool expected)
    {
        var asOf = new DateOnly(2026, 1, 31);
        var dueDate = asOf.AddDays(-daysPastDue);
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), dueDate.AddDays(-30), "test", dueDate: dueDate));
        var item = account.OpenItems.Single();

        var spec = new IsOverdue(asOf, graceDays);

        Assert.Equal(expected, spec.IsSatisfiedBy(item));
    }
}
