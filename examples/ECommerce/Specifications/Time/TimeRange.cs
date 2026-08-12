namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Time;

/// <summary>
/// Specification for checking if a time is within a specific range (e.g. flash sale hours).
/// </summary>
/// <param name="startTime">Start time, e.g. "09:00".</param>
/// <param name="endTime">End time, e.g. "17:00".</param>
public sealed class TimeRange(TimeOnly startTime, TimeOnly endTime) : Specification<DateTimeOffset>
{
    public override bool IsSatisfiedBy(DateTimeOffset candidate)
    {
        var currentTime = TimeOnly.FromDateTime(candidate.DateTime);

        // Handle time ranges that span midnight (e.g., 22:00 to 02:00).
        if (startTime > endTime)
        {
            return currentTime >= startTime || currentTime <= endTime;
        }

        return currentTime >= startTime && currentTime <= endTime;
    }
}
