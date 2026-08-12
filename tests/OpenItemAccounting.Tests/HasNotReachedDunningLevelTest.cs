using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;
using Xunit;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Tests;

public sealed class HasNotReachedDunningLevelTest
{
    [Theory]
    [InlineData(DunningLevel.None, DunningLevel.FirstDunning, true)]
    [InlineData(DunningLevel.FirstDunning, DunningLevel.FirstDunning, false)]
    [InlineData(DunningLevel.SecondDunning, DunningLevel.FirstDunning, false)]
    public void IsSatisfiedBy_EvaluatesCurrentLevelAgainstTarget(
        DunningLevel currentLevel, DunningLevel targetLevel, bool expected)
    {
        var asOf = new DateOnly(2026, 1, 31);
        var account = new LedgerAccount("ACC-1", "Test Customer", CustomerRiskClass.Standard, "EUR");
        account.Post(new Posting("INV-1", PostingType.Invoice, new Money(100m, "EUR"), asOf.AddDays(-30), "test", dueDate: asOf));

        if (currentLevel != DunningLevel.None)
        {
            account.EscalateDunning("INV-1", currentLevel, asOf);
        }

        var item = account.OpenItems.Single();
        var spec = new HasNotReachedDunningLevel(targetLevel);

        Assert.Equal(expected, spec.IsSatisfiedBy(item));
    }
}
