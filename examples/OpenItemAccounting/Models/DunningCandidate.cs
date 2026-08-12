namespace Phauthentic.Specification.Examples.OpenItemAccounting.Models;

/// <summary>
/// Evaluation context pairing an open item with the account it belongs to, as of a given date.
/// Lets dunning specifications reason about account-level and item-level rules together,
/// the same way the ECommerce example's <c>Order</c> carries its <c>Customer</c>.
/// </summary>
public sealed record DunningCandidate(LedgerAccount Account, OpenItem OpenItem, DateOnly AsOf);
