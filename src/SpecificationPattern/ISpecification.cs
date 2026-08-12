namespace Phauthentic.Specification;

/// <summary>
/// Specification Interface.
/// </summary>
/// <remarks>
/// See <see href="https://en.wikipedia.org/wiki/Specification_pattern"/> and
/// <see href="http://www.martinfowler.com/apsupp/spec.pdf"/>.
/// </remarks>
/// <typeparam name="T">The type of candidate this specification evaluates.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Checks whether the given candidate satisfies this specification.
    /// </summary>
    bool IsSatisfiedBy(T candidate);

    ISpecification<T> And(ISpecification<T> other);

    ISpecification<T> AndNot(ISpecification<T> other);

    ISpecification<T> Or(ISpecification<T> other);

    ISpecification<T> OrNot(ISpecification<T> other);

    /// <summary>
    /// Returns a specification that is satisfied when this specification is not.
    /// </summary>
    ISpecification<T> Not();
}
