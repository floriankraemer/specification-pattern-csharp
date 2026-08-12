namespace Phauthentic.Specification.Examples.ECommerce.Tests;

/// <summary>
/// Test helper specification that delegates to an arbitrary predicate.
/// </summary>
internal sealed class ClosureSpecification<T>(Func<T, bool> predicate) : Specification<T>
{
    public override bool IsSatisfiedBy(T candidate) => predicate(candidate);
}
