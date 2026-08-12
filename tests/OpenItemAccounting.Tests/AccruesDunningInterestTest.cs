using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Dunning;
using Xunit;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Tests;

public sealed class AccruesDunningInterestTest
{
    private static readonly DateOnly AsOf = new(2026, 1, 31);

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenAllConditionsAreMet()
    {
        var candidate = CreateCandidate(overdueDays: 46, amount: 100m, currentLevel: DunningLevel.FirstDunning);

        Assert.True(new AccruesDunningInterest().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenBelowFirstDunningLevel()
    {
        var candidate = CreateCandidate(overdueDays: 46, amount: 100m, currentLevel: DunningLevel.None);

        Assert.False(new AccruesDunningInterest().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenNotYetOverdueBeyondGrace()
    {
        var candidate = CreateCandidate(overdueDays: 45, amount: 100m, currentLevel: DunningLevel.FirstDunning);

        Assert.False(new AccruesDunningInterest().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAmountBelowMinimum()
    {
        var candidate = CreateCandidate(overdueDays: 46, amount: 99.99m, currentLevel: DunningLevel.FirstDunning);

        Assert.False(new AccruesDunningInterest().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsDisputed()
    {
        var candidate = CreateCandidate(overdueDays: 46, amount: 100m, currentLevel: DunningLevel.FirstDunning, disputed: true);

        Assert.False(new AccruesDunningInterest().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsNotOpen()
    {
        var candidate = CreateCandidate(overdueDays: 46, amount: 100m, currentLevel: DunningLevel.FirstDunning, writeOff: true);

        Assert.False(new AccruesDunningInterest().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_ForPreferredCustomer_EvenWhenOtherwiseEligible()
    {
        var candidate = CreateCandidate(overdueDays: 46, amount: 100m, currentLevel: DunningLevel.FirstDunning, riskClass: CustomerRiskClass.Preferred);

        Assert.False(new AccruesDunningInterest().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAccountIsBlockedForDunning()
    {
        var candidate = CreateCandidate(overdueDays: 46, amount: 100m, currentLevel: DunningLevel.FirstDunning, blocked: true);

        Assert.False(new AccruesDunningInterest().IsSatisfiedBy(candidate));
    }

    private static DunningCandidate CreateCandidate(
        int overdueDays,
        decimal amount,
        DunningLevel currentLevel,
        bool disputed = false,
        bool blocked = false,
        bool writeOff = false,
        CustomerRiskClass riskClass = CustomerRiskClass.Standard)
    {
        var dueDate = AsOf.AddDays(-overdueDays);
        var account = new LedgerAccount("ACC-1", "Test Customer", riskClass, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(amount, "EUR"), dueDate.AddDays(-30), "test", dueDate: dueDate));

        if (disputed)
        {
            account.DisputeOpenItem("INV-1", true);
        }

        if (writeOff)
        {
            account.Post(new Posting("WO-1", PostingType.WriteOff, new Money(amount, "EUR"), AsOf, "write off", referenceOpenItemId: "INV-1"));
        }

        if (currentLevel != DunningLevel.None)
        {
            account.EscalateDunning("INV-1", currentLevel, AsOf.AddDays(-1));
        }

        if (blocked)
        {
            account.Block();
        }

        var item = account.OpenItems.Single();

        return new DunningCandidate(account, item, AsOf);
    }
}
