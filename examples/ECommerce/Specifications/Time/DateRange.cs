namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Time;

/// <summary>
/// Specification for checking if a date is within a specific range (inclusive, until end of day).
/// </summary>
public sealed class DateRange(DateOnly startDate, DateOnly endDate) : Specification<DateTimeOffset>
{
    public override bool IsSatisfiedBy(DateTimeOffset candidate)
    {
        var start = new DateTimeOffset(startDate.ToDateTime(TimeOnly.MinValue), candidate.Offset);
        var end = new DateTimeOffset(endDate.ToDateTime(new TimeOnly(23, 59, 59)), candidate.Offset);

        return candidate >= start && candidate <= end;
    }
}
