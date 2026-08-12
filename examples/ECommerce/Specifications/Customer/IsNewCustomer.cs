using Phauthentic.Specification.Examples.ECommerce.Models;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;

/// <summary>
/// Specification for checking if an order's customer is new (account created recently).
/// </summary>
public sealed class IsNewCustomer(int maxAgeInDays = 30) : Specification<Models.Order>
{
    public override bool IsSatisfiedBy(Models.Order candidate) => candidate.Customer.GetAccountAgeInDays() <= maxAgeInDays;
}
