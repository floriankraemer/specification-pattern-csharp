namespace Phauthentic.Specification.Examples.OpenItemAccounting.Models;

/// <summary>
/// The kind of ledger entry being posted to an account.
/// </summary>
public enum PostingType
{
    /// <summary>Creates a new open item (e.g. a customer invoice).</summary>
    Invoice,

    /// <summary>Fully or partially clears an existing open item.</summary>
    Payment,

    /// <summary>Reduces an existing open item without a cash payment.</summary>
    CreditMemo,

    /// <summary>Removes an open item from collections (bad debt).</summary>
    WriteOff,
}
