namespace Phauthentic.Specification;

public sealed class NotSpecification<T>(ISpecification<T> wrapped) : Specification<T>
{
    public override bool IsSatisfiedBy(T candidate) => !wrapped.IsSatisfiedBy(candidate);
}
