namespace Phauthentic.Specification;

/// <summary>
/// Base class for specifications, providing the boolean combinator methods.
/// </summary>
/// <typeparam name="T">The type of candidate this specification evaluates.</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    public abstract bool IsSatisfiedBy(T candidate);

    public ISpecification<T> And(ISpecification<T> other) => new AndSpecification<T>(this, other);

    public ISpecification<T> AndNot(ISpecification<T> other) => new AndNotSpecification<T>(this, other);

    public ISpecification<T> Or(ISpecification<T> other) => new OrSpecification<T>(this, other);

    public ISpecification<T> OrNot(ISpecification<T> other) => new OrNotSpecification<T>(this, other);

    public ISpecification<T> Not() => new NotSpecification<T>(this);
}
