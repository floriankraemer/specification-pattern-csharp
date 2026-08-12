using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Dunning;
using Xunit;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Tests;

public sealed class DunningLevelEligibilityTest
{
    private static readonly DateOnly AsOf = new(2026, 1, 31);

    public static TheoryData<DunningLevel, int, decimal> LevelRules => new()
    {
        { DunningLevel.FriendlyReminder, 7, 1m },
        { DunningLevel.FirstDunning, 21, 25m },
        { DunningLevel.SecondDunning, 35, 50m },
        { DunningLevel.FinalDunning, 49, 100m },
    };

    [Theory]
    [MemberData(nameof(LevelRules))]
    public void IsSatisfiedBy_ReturnsTrue_WhenOverdueBeyondGraceWithSufficientAmount(
        DunningLevel level, int graceDays, decimal minimumAmount)
    {
        var candidate = CreateCandidate(overdueDays: graceDays + 1, amount: minimumAmount);
        var spec = new DunningLevelEligibility(level, graceDays, minimumAmount);

        Assert.True(spec.IsSatisfiedBy(candidate));
    }

    [Theory]
    [MemberData(nameof(LevelRules))]
    public void IsSatisfiedBy_ReturnsFalse_WhenNotYetOverdueBeyondGrace(
        DunningLevel level, int graceDays, decimal minimumAmount)
    {
        var candidate = CreateCandidate(overdueDays: graceDays, amount: minimumAmount);
        var spec = new DunningLevelEligibility(level, graceDays, minimumAmount);

        Assert.False(spec.IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAmountBelowMinimum()
    {
        var candidate = CreateCandidate(overdueDays: 22, amount: 24.99m);
        var spec = new DunningLevelEligibility(DunningLevel.FirstDunning, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsDisputed()
    {
        var candidate = CreateCandidate(overdueDays: 22, amount: 25m, disputed: true);
        var spec = new DunningLevelEligibility(DunningLevel.FirstDunning, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemAlreadyAtTargetLevel()
    {
        var candidate = CreateCandidate(overdueDays: 22, amount: 25m, currentLevel: DunningLevel.FirstDunning);
        var spec = new DunningLevelEligibility(DunningLevel.FirstDunning, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsNotOpen()
    {
        var candidate = CreateCandidate(overdueDays: 22, amount: 25m, writeOff: true);
        var spec = new DunningLevelEligibility(DunningLevel.FirstDunning, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAccountIsBlockedForDunning()
    {
        var candidate = CreateCandidate(overdueDays: 22, amount: 25m, blocked: true);
        var spec = new DunningLevelEligibility(DunningLevel.FirstDunning, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_ForPreferredCustomer_WithinExtendedGracePeriod()
    {
        var candidate = CreateCandidate(overdueDays: 25, amount: 25m, riskClass: CustomerRiskClass.Preferred);
        var spec = new DunningLevelEligibility(DunningLevel.FirstDunning, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(candidate));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_ForHighRiskCustomer_BeforeStandardGracePeriodEnds()
    {
        var candidate = CreateCandidate(overdueDays: 15, amount: 25m, riskClass: CustomerRiskClass.HighRisk);
        var spec = new DunningLevelEligibility(DunningLevel.FirstDunning, 21, 25m);

        Assert.True(spec.IsSatisfiedBy(candidate));
    }

    [Theory]
    [InlineData(14, false)]
    [InlineData(15, true)]
    public void IsSatisfiedBy_ForHighRiskCustomer_RespectsAdjustedGracePeriodBoundary(
        int overdueDays, bool expected)
    {
        var candidate = CreateCandidate(overdueDays: overdueDays, amount: 25m, riskClass: CustomerRiskClass.HighRisk);
        var spec = new DunningLevelEligibility(DunningLevel.FirstDunning, 21, 25m);

        Assert.Equal(expected, spec.IsSatisfiedBy(candidate));
    }

    private static DunningCandidate CreateCandidate(
        int overdueDays,
        decimal amount,
        bool disputed = false,
        bool blocked = false,
        bool writeOff = false,
        DunningLevel currentLevel = DunningLevel.None,
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
