namespace Phauthentic.Specification;

public sealed class OrNotSpecification<T>(ISpecification<T> left, ISpecification<T> right) : Specification<T>
{
    public override bool IsSatisfiedBy(T candidate) =>
        left.IsSatisfiedBy(candidate) || !right.IsSatisfiedBy(candidate);
}
