using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Account;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Dunning;

/// <summary>
/// Dunning-level escalation rule.
/// </summary>
/// <remarks>
/// Business rules:
/// <list type="bullet">
/// <item>The item must still be open and not disputed</item>
/// <item>The item must not already be at or above the target level</item>
/// <item>The remaining balance must meet the level's minimum amount</item>
/// <item>The item must be overdue by more than the level's grace period</item>
/// <item>Preferred customers get 14 extra grace days; high-risk customers get 7 fewer</item>
/// <item>The account must not be blocked for dunning</item>
/// </list>
/// One parameterized class replaces four near-identical per-level classes.
/// </remarks>
public sealed class DunningLevelEligibility(DunningLevel targetLevel, int graceDays, decimal minimumAmount)
    : Specification<Models.DunningCandidate>
{
    public override bool IsSatisfiedBy(Models.DunningCandidate candidate)
    {
        var riskAdjustment = candidate.Account.RiskClass switch
        {
            CustomerRiskClass.Preferred => 14,
            CustomerRiskClass.HighRisk => -7,
            _ => 0,
        };

        var itemSpec = new IsOpen()
            .And(new IsNotDisputed())
            .And(new HasNotReachedDunningLevel(targetLevel))
            .And(new MinimumOpenAmount(minimumAmount))
            .And(new IsOverdue(candidate.AsOf, graceDays + riskAdjustment));

        var accountSpec = new IsNotBlockedForDunning();

        return itemSpec.IsSatisfiedBy(candidate.OpenItem) && accountSpec.IsSatisfiedBy(candidate.Account);
    }
}
