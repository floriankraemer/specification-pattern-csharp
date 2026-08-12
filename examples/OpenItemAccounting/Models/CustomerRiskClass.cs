namespace Phauthentic.Specification.Examples.OpenItemAccounting.Models;

/// <summary>
/// Risk classification of a customer account, used to adjust dunning leniency.
/// </summary>
public enum CustomerRiskClass
{
    Standard,
    Preferred,
    HighRisk,
}
