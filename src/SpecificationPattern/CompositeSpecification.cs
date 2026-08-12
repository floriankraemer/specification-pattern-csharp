namespace Phauthentic.Specification;

/// <summary>
/// A specification that is satisfied only when all of the given specifications are satisfied.
/// </summary>
/// <typeparam name="T">The type of candidate this specification evaluates.</typeparam>
public sealed class CompositeSpecification<T> : Specification<T>
{
    private readonly List<ISpecification<T>> _specifications = [];

    /// <param name="specifications">The specifications that must all be satisfied.</param>
    /// <exception cref="ArgumentException">Thrown when a null specification is supplied.</exception>
    public CompositeSpecification(IEnumerable<ISpecification<T>> specifications)
    {
        foreach (var specification in specifications)
        {
            if (specification is null)
            {
                throw new ArgumentException(
                    $"Null is not an instance of {typeof(ISpecification<T>)}",
                    nameof(specifications));
            }

            _specifications.Add(specification);
        }
    }

    public override bool IsSatisfiedBy(T candidate) => _specifications.All(s => s.IsSatisfiedBy(candidate));
}
