using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Account;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Dunning;

/// <summary>
/// Dunning interest accrual rule.
/// </summary>
/// <remarks>
/// Business rules:
/// <list type="bullet">
/// <item>The item must already be at First Dunning level or beyond</item>
/// <item>The item must be open, not disputed, and overdue by more than 45 days</item>
/// <item>The remaining balance must be at least 100</item>
/// <item>Preferred customers are exempt from interest, regardless of the above</item>
/// <item>The account must not be blocked for dunning</item>
/// </list>
/// </remarks>
public sealed class AccruesDunningInterest : Specification<Models.DunningCandidate>
{
    private const int InterestGraceDays = 45;
    private const decimal MinimumInterestAmount = 100m;

    public override bool IsSatisfiedBy(Models.DunningCandidate candidate)
    {
        var itemSpec = new IsOpen()
            .And(new IsNotDisputed())
            .And(new MinimumOpenAmount(MinimumInterestAmount))
            .And(new IsOverdue(candidate.AsOf, InterestGraceDays));

        var accountSpec = new IsNotBlockedForDunning()
            .AndNot(new RiskClass(CustomerRiskClass.Preferred));

        return candidate.OpenItem.CurrentDunningLevel >= DunningLevel.FirstDunning
            && itemSpec.IsSatisfiedBy(candidate.OpenItem)
            && accountSpec.IsSatisfiedBy(candidate.Account);
    }
}
