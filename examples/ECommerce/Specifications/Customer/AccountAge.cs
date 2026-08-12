using Phauthentic.Specification.Examples.ECommerce.Models;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;

public enum AccountAgeComparison
{
    Minimum,
    Maximum,
}

/// <summary>
/// Specification for checking an order's customer's account age.
/// </summary>
public sealed class AccountAge(int thresholdDays, AccountAgeComparison comparison = AccountAgeComparison.Minimum)
    : Specification<Models.Order>
{
    public override bool IsSatisfiedBy(Models.Order candidate)
    {
        var accountAge = candidate.Customer.GetAccountAgeInDays();

        return comparison switch
        {
            AccountAgeComparison.Minimum => accountAge >= thresholdDays,
            AccountAgeComparison.Maximum => accountAge <= thresholdDays,
            _ => false,
        };
    }
}
