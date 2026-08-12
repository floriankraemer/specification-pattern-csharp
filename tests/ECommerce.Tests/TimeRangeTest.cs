using Phauthentic.Specification.Examples.ECommerce.Specifications.Time;
using Xunit;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

public sealed class TimeRangeTest
{
    [Theory]
    [InlineData(8, 59, 59, false)]
    [InlineData(9, 0, 0, true)]
    [InlineData(9, 0, 1, true)]
    [InlineData(17, 0, 0, true)]
    [InlineData(17, 0, 1, false)]
    public void IsSatisfiedBy_ReturnsExpectedResultAtRangeBoundaries(int hour, int minute, int second, bool expected)
    {
        var candidate = new DateTimeOffset(2024, 1, 10, hour, minute, second, TimeSpan.Zero);

        var spec = new TimeRange(new TimeOnly(9, 0), new TimeOnly(17, 0));

        Assert.Equal(expected, spec.IsSatisfiedBy(candidate));
    }

    [Theory]
    [InlineData(22, 0, 0, true)]
    [InlineData(23, 0, 0, true)]
    [InlineData(2, 0, 0, true)]
    [InlineData(1, 59, 59, true)]
    [InlineData(12, 0, 0, false)]
    public void IsSatisfiedBy_ReturnsExpectedResultWhenRangeSpansMidnight(
        int hour, int minute, int second, bool expected)
    {
        var candidate = new DateTimeOffset(2024, 1, 10, hour, minute, second, TimeSpan.Zero);

        var spec = new TimeRange(new TimeOnly(22, 0), new TimeOnly(2, 0));

        Assert.Equal(expected, spec.IsSatisfiedBy(candidate));
    }
}
