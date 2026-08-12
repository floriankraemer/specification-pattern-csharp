using Phauthentic.Specification.Examples.OpenItemAccounting.Models;

namespace Phauthentic.Specification.Examples.OpenItemAccounting;

/// <summary>
/// Builds the sample <see cref="LedgerAccount"/>s used by the demo, each isolating one dunning rule.
/// </summary>
internal static class SampleAccountFactory
{
    private const string Currency = "EUR";

    public static List<LedgerAccount> CreateSampleAccounts(DateOnly asOf)
    {
        // Account 1: Standard customer, 40 days overdue - should escalate to Second Dunning.
        var acme = new LedgerAccount(
            accountNumber: "10045",
            customerName: "Acme Manufacturing GmbH",
            riskClass: CustomerRiskClass.Standard,
            currency: Currency);

        acme.Post(new Posting(
            id: "INV-1001",
            type: PostingType.Invoice,
            amount: new Money(500m, Currency),
            postingDate: asOf.AddDays(-45),
            description: "Delivery of industrial parts",
            dueDate: asOf.AddDays(-40)));

        // Account 2: Preferred customer, same 40 days overdue - risk-based leniency limits this to First Dunning.
        var nordic = new LedgerAccount(
            accountNumber: "10046",
            customerName: "Nordic Retail Group",
            riskClass: CustomerRiskClass.Preferred,
            currency: Currency);

        nordic.Post(new Posting(
            id: "INV-1002",
            type: PostingType.Invoice,
            amount: new Money(500m, Currency),
            postingDate: asOf.AddDays(-45),
            description: "Quarterly stock replenishment",
            dueDate: asOf.AddDays(-40)));

        // Account 3: High-risk customer, only 10 days overdue - risk-based penalty escalates it early.
        var fastFashion = new LedgerAccount(
            accountNumber: "10047",
            customerName: "Fast Fashion Express",
            riskClass: CustomerRiskClass.HighRisk,
            currency: Currency);

        fastFashion.Post(new Posting(
            id: "INV-1003",
            type: PostingType.Invoice,
            amount: new Money(50m, Currency),
            postingDate: asOf.AddDays(-15),
            description: "Sample order",
            dueDate: asOf.AddDays(-10)));

        // Account 4: Standard customer, 60 days overdue and disputed - dispute halts every rule.
        var bergmann = new LedgerAccount(
            accountNumber: "10048",
            customerName: "Bergmann Logistics",
            riskClass: CustomerRiskClass.Standard,
            currency: Currency);

        bergmann.Post(new Posting(
            id: "INV-1004",
            type: PostingType.Invoice,
            amount: new Money(1000m, Currency),
            postingDate: asOf.AddDays(-65),
            description: "Freight forwarding services",
            dueDate: asOf.AddDays(-60)));
        bergmann.DisputeOpenItem(openItemId: "INV-1004", disputed: true);

        // Account 5: Standard customer already at Final Dunning - triggers interest accrual and legal referral.
        var continental = new LedgerAccount(
            accountNumber: "10049",
            customerName: "Continental Foods Ltd",
            riskClass: CustomerRiskClass.Standard,
            currency: Currency);

        continental.Post(new Posting(
            id: "INV-1005",
            type: PostingType.Invoice,
            amount: new Money(5000m, Currency),
            postingDate: asOf.AddDays(-60),
            description: "Bulk raw material supply",
            dueDate: asOf.AddDays(-55)));

        continental.EscalateDunning(openItemId: "INV-1005", level: DunningLevel.FinalDunning, runDate: asOf.AddDays(-20));

        // Account 6: Standard customer, blocked for dunning (e.g. insolvency proceedings) - blocks every rule.
        var vantage = new LedgerAccount(
            accountNumber: "10050",
            customerName: "Vantage Industrial Supply",
            riskClass: CustomerRiskClass.Standard,
            currency: Currency);

        vantage.Post(new Posting(
            id: "INV-1006",
            type: PostingType.Invoice,
            amount: new Money(2000m, Currency),
            postingDate: asOf.AddDays(-95),
            description: "Machinery parts order",
            dueDate: asOf.AddDays(-90)));

        vantage.Block();

        return [acme, nordic, fastFashion, bergmann, continental, vantage];
    }
}
