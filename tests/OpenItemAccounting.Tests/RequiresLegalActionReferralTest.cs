using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Dunning;
using Xunit;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Tests;

public sealed class RequiresLegalActionReferralTest
{
    private static readonly DateOnly AsOf = new(2026, 1, 31);

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenAllConditionsAreMet()
    {
        var candidate = CreateCandidate(amount: 250m, daysSinceLastDunningRun: 15);

        Assert.True(new RequiresLegalActionReferral().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenNotAtFinalDunningLevel()
    {
        var candidate = CreateCandidate(amount: 250m, daysSinceLastDunningRun: 15, currentLevel: DunningLevel.SecondDunning);

        Assert.False(new RequiresLegalActionReferral().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenNotYetOverdueSinceLastDunningRun()
    {
        var candidate = CreateCandidate(amount: 250m, daysSinceLastDunningRun: 14);

        Assert.False(new RequiresLegalActionReferral().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAmountBelowMinimum()
    {
        var candidate = CreateCandidate(amount: 249.99m, daysSinceLastDunningRun: 15);

        Assert.False(new RequiresLegalActionReferral().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsDisputed()
    {
        var candidate = CreateCandidate(amount: 250m, daysSinceLastDunningRun: 15, disputed: true);

        Assert.False(new RequiresLegalActionReferral().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsNotOpen()
    {
        var candidate = CreateCandidate(amount: 250m, daysSinceLastDunningRun: 15, writeOff: true);

        Assert.False(new RequiresLegalActionReferral().IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAccountIsBlockedForDunning()
    {
        var candidate = CreateCandidate(amount: 250m, daysSinceLastDunningRun: 15, blocked: true);

        Assert.False(new RequiresLegalActionReferral().IsSatisfiedBy(candidate));
    }

    private static DunningCandidate CreateCandidate(
        decimal amount,
        int daysSinceLastDunningRun,
        DunningLevel currentLevel = DunningLevel.FinalDunning,
        bool disputed = false,
        bool blocked = false,
        bool writeOff = false)
    {
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(amount, "EUR"), AsOf.AddDays(-90), "test", dueDate: AsOf.AddDays(-80)));
        account.EscalateDunning("INV-1", currentLevel, AsOf.AddDays(-daysSinceLastDunningRun));

        if (disputed)
        {
            account.DisputeOpenItem("INV-1", true);
        }

        if (writeOff)
        {
            account.Post(new Posting("WO-1", PostingType.WriteOff, new Money(amount, "EUR"), AsOf, "write off", referenceOpenItemId: "INV-1"));
        }

        if (blocked)
        {
            account.Block();
        }

        var item = account.OpenItems.Single();

        return new DunningCandidate(account, item, AsOf);
    }
}
