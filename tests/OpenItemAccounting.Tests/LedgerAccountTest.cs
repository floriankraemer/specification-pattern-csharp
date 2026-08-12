using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Xunit;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Tests;

public sealed class LedgerAccountTest
{
    private static readonly DateOnly AsOf = new(2026, 1, 31);

    [Fact]
    public void Post_Invoice_CreatesOpenOpenItem()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");

        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), AsOf.AddDays(-10), "test", dueDate: AsOf));

        var item = Assert.Single(account.OpenItems);
        Assert.Equal("INV-1", item.Id);
        Assert.Equal(OpenItemStatus.Open, item.Status);
        Assert.Equal(DunningLevel.None, item.CurrentDunningLevel);
        Assert.Equal(new Money(100m, "EUR"), item.OpenAmount);
        Assert.Equal(AsOf, item.DueDate);
    }

    [Fact]
    public void Post_FullPayment_ClearsOpenItem()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), AsOf.AddDays(-10), "test", dueDate: AsOf));

        account.Post(new Posting("PAY-1", PostingType.Payment, new Money(100m, "EUR"), AsOf, "payment", referenceOpenItemId: "INV-1"));

        var item = Assert.Single(account.OpenItems);
        Assert.Equal(OpenItemStatus.Cleared, item.Status);
        Assert.Equal(new Money(0m, "EUR"), item.OpenAmount);
    }

    [Fact]
    public void Post_PartialPayment_ReducesOpenAmountButKeepsItemOpen()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), AsOf.AddDays(-10), "test", dueDate: AsOf));

        account.Post(new Posting("PAY-1", PostingType.Payment, new Money(40m, "EUR"), AsOf, "payment", referenceOpenItemId: "INV-1"));

        var item = Assert.Single(account.OpenItems);
        Assert.Equal(OpenItemStatus.Open, item.Status);
        Assert.Equal(new Money(60m, "EUR"), item.OpenAmount);
    }

    [Fact]
    public void Post_CreditMemo_ReducesOpenAmount()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), AsOf.AddDays(-10), "test", dueDate: AsOf));

        account.Post(new Posting("CM-1", PostingType.CreditMemo, new Money(30m, "EUR"), AsOf, "credit memo", referenceOpenItemId: "INV-1"));

        var item = Assert.Single(account.OpenItems);
        Assert.Equal(new Money(70m, "EUR"), item.OpenAmount);
    }

    [Fact]
    public void Post_WriteOff_SetsStatusWrittenOff()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), AsOf.AddDays(-10), "test", dueDate: AsOf));

        account.Post(new Posting("WO-1", PostingType.WriteOff, new Money(100m, "EUR"), AsOf, "write off", referenceOpenItemId: "INV-1"));

        var item = Assert.Single(account.OpenItems);
        Assert.Equal(OpenItemStatus.WrittenOff, item.Status);
    }

    [Fact]
    public void Post_WithMismatchedCurrency_Throws()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");

        Assert.Throws<InvalidOperationException>(() =>
            account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "USD"), AsOf.AddDays(-10), "test", dueDate: AsOf)));
    }

    [Fact]
    public void Post_PaymentForUnknownOpenItem_Throws()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");

        Assert.Throws<InvalidOperationException>(() =>
            account.Post(new Posting("PAY-1", PostingType.Payment, new Money(100m, "EUR"), AsOf, "payment", referenceOpenItemId: "INV-1")));
    }

    [Fact]
    public void DisputeOpenItem_SetsIsDisputed()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), AsOf.AddDays(-10), "test", dueDate: AsOf));

        account.DisputeOpenItem("INV-1", true);
        Assert.True(account.OpenItems.Single().IsDisputed);

        account.DisputeOpenItem("INV-1", false);
        Assert.False(account.OpenItems.Single().IsDisputed);
    }

    [Fact]
    public void DisputeOpenItem_WithUnknownOpenItemId_Throws()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");

        Assert.Throws<InvalidOperationException>(() => account.DisputeOpenItem("INV-1", true));
    }

    [Fact]
    public void EscalateDunning_SetsLevelAndLastDunningDate()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), AsOf.AddDays(-10), "test", dueDate: AsOf));

        account.EscalateDunning("INV-1", DunningLevel.SecondDunning, AsOf);

        var item = account.OpenItems.Single();
        Assert.Equal(DunningLevel.SecondDunning, item.CurrentDunningLevel);
        Assert.Equal(AsOf, item.LastDunningDate);
    }

    [Fact]
    public void EscalateDunning_WithUnknownOpenItemId_Throws()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");

        Assert.Throws<InvalidOperationException>(() => account.EscalateDunning("INV-1", DunningLevel.FirstDunning, AsOf));
    }

    [Fact]
    public void Block_SetsIsBlockedForDunning()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");

        account.Block();

        Assert.True(account.IsBlockedForDunning);
    }

    [Fact]
    public void Unblock_ClearsIsBlockedForDunning()
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Block();

        account.Unblock();

        Assert.False(account.IsBlockedForDunning);
    }
}
