using Phauthentic.Specification.Examples.ECommerce.Specifications.Time;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class DateRangeTest
{
    private static readonly DateOnly StartDate = new(2024, 1, 10);
    private static readonly DateOnly EndDate = new(2024, 1, 15);

    [Theory]
    [InlineData(2024, 1, 9, 23, 59, 59, false)]
    [InlineData(2024, 1, 10, 0, 0, 0, true)]
    [InlineData(2024, 1, 10, 0, 0, 1, true)]
    [InlineData(2024, 1, 15, 23, 59, 59, true)]
    [InlineData(2024, 1, 16, 0, 0, 0, false)]
    public void IsSatisfiedBy_ReturnsExpectedResultAtRangeBoundaries(
        int year, int month, int day, int hour, int minute, int second, bool expected)
    {
        var candidate = new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero);

        var spec = new DateRange(StartDate, EndDate);

        Assert.Equal(expected, spec.IsSatisfiedBy(candidate));
    }
}
