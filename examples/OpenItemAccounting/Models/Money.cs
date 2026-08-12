namespace Phauthentic.Specification.Examples.OpenItemAccounting.Models;

/// <summary>
/// Value object representing a monetary amount in a specific currency.
/// </summary>
public readonly record struct Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency) => new(0m, currency);

    public Money Add(Money other) => Combine(other, static (a, b) => a + b);

    public Money Subtract(Money other) => Combine(other, static (a, b) => a - b);

    public static bool operator <(Money left, Money right) => Compare(left, right) < 0;

    public static bool operator >(Money left, Money right) => Compare(left, right) > 0;

    public static bool operator <=(Money left, Money right) => Compare(left, right) <= 0;

    public static bool operator >=(Money left, Money right) => Compare(left, right) >= 0;

    private Money Combine(Money other, Func<decimal, decimal, decimal> operation)
    {
        RequireSameCurrency(other);

        return new Money(operation(Amount, other.Amount), Currency);
    }

    private static int Compare(Money left, Money right)
    {
        left.RequireSameCurrency(right);

        return left.Amount.CompareTo(right.Amount);
    }

    private void RequireSameCurrency(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException($"Currency mismatch: {Currency} vs {other.Currency}");
        }
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
