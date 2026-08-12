using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;
using Xunit;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Tests;

public sealed class MinimumOpenAmountTest
{
    [Theory]
    [InlineData(100, 0, 100, true)]
    [InlineData(100, 0, 100.01, false)]
    [InlineData(100, 100, 0, true)]
    [InlineData(100, 150, 0, false)]
    [InlineData(100, 150, -100, true)]
    public void IsSatisfiedBy_EvaluatesOpenAmountAgainstThreshold(
        decimal invoiceAmount, decimal paymentAmount, decimal minimumAmount, bool expected)
    {
        var asOf = new DateOnly(2026, 1, 31);
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(invoiceAmount, "EUR"), asOf.AddDays(-30), "test", dueDate: asOf));

        if (paymentAmount != 0)
        {
            account.Post(new Posting("PAY-1", PostingType.Payment, new Money(paymentAmount, "EUR"), asOf, "payment", referenceOpenItemId: "INV-1"));
        }

        var item = account.OpenItems.Single();
        var spec = new MinimumOpenAmount(minimumAmount);

        Assert.Equal(expected, spec.IsSatisfiedBy(item));
    }
}
