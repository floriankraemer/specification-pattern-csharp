# Specifications and Repositories

A common question when using the Specification Pattern in a Domain-Driven Design context: can a specification take a repository (or its interface) as a dependency, so it can look up data from the database while evaluating `IsSatisfiedBy`?

**No.** Injecting a repository into a specification is an anti-pattern.

It's convenient in the short term but breaks two things that make specifications valuable in the first place: domain-model purity and separation of concerns.

## Why It's a Problem

### 1. It Breaks Domain-Model Purity

A specification answers: "does this candidate, as given, satisfy this rule?"

That's a pure function of its input.

The moment `IsSatisfiedBy` calls into a repository, the domain layer picks up an out-of-process dependency — I/O, a database round-trip, a network call — hidden behind what looks like a plain boolean check.
Nothing at the call site signals that `spec.IsSatisfiedBy(candidate)` might hit a database.

### 2. It Creates Hidden I/O and Performance Problems

`IsSatisfiedBy` is expected to be fast, deterministic, and side-effect-free — that's why it's safe to call it inside a `.Where()` over a whole collection, as this repository's examples do:

```csharp
var itemSpec = new IsOpen().And(new IsNotDisputed()).And(new IsOverdue(asOf, graceDays: 21));

foreach (var item in account.OpenItems.Where(itemSpec.IsSatisfiedBy))
{
    // ...
}
```

If `IsOpen` or `IsOverdue` triggered a database call internally, this loop becomes N database round-trips — a classic N+1 problem, invisible at the call site.

Async repositories make it worse: `IsSatisfiedBy` is synchronous by design (see [`ISpecification<T>`](../src/SpecificationPattern/ISpecification.cs)), so an async DB call inside it forces `.Result` or `.Wait()`, which risks deadlocks and blocks a thread pool thread for the duration of the query.

### 3. It Makes Testing Harder for No Reason

This repository's specifications are trivial to unit test: construct a candidate, call `IsSatisfiedBy`, assert.
See [`SpecificationTest.cs`](../tests/SpecificationPattern.Tests/SpecificationTest.cs) for the pattern — no mocks, no setup, no I/O.
A repository-backed specification needs a mocked or in-memory repository just to test a rule that has nothing to do with persistence, and every consumer of that specification inherits the same mocking burden.

## What to Do Instead

The fix is always the same shape: fetch what the rule needs *before* you evaluate it, and pass that data in.

Where you do the fetching depends on how far the data has to travel.

### Option A — Pass the Data Directly

If a rule needs one extra fact, add it as a constructor parameter, the way this repository's own specifications do it:

```csharp
namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;

/// <summary>
/// Satisfied when an open item is overdue by more than <paramref name="graceDays"/> as of <paramref name="asOf"/>.
/// </summary>
public sealed class IsOverdue(DateOnly asOf, int graceDays) : Specification<Models.OpenItem>
{
    public override bool IsSatisfiedBy(Models.OpenItem candidate) => candidate.DaysOverdue(asOf) > graceDays;
}
```

`asOf` isn't looked up inside the specification — the caller (the application layer, or a test) decides what "now" means and passes it in.
No repository, no I/O, no hidden dependency.

### Option B — Pass a Pre-Fetched Evaluation Context

If a rule genuinely spans more than one aggregate, pre-fetch everything it needs into a small context object and specify against *that*, instead of reaching out mid-evaluation.
This is exactly what `DunningCandidate` does in the `OpenItemAccounting` example:

```csharp
namespace Phauthentic.Specification.Examples.OpenItemAccounting.Models;

/// <summary>
/// Evaluation context pairing an open item with the account it belongs to, as of a given date.
/// Lets dunning specifications reason about account-level and item-level rules together.
/// </summary>
public sealed record DunningCandidate(LedgerAccount Account, OpenItem OpenItem, DateOnly AsOf);
```

```csharp
namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Dunning;

public sealed class AccruesDunningInterest : Specification<Models.DunningCandidate>
{
    public override bool IsSatisfiedBy(Models.DunningCandidate candidate)
    {
        var itemSpec = new IsOpen().And(new IsNotDisputed()).And(new MinimumOpenAmount(100m));
        var accountSpec = new IsNotBlockedForDunning().AndNot(new RiskClass(CustomerRiskClass.Preferred));

        return itemSpec.IsSatisfiedBy(candidate.OpenItem) && accountSpec.IsSatisfiedBy(candidate.Account);
    }
}
```

The `LedgerAccount` and `OpenItem` are already loaded by the caller before `AccruesDunningInterest` ever runs — see [`Program.cs`](../examples/OpenItemAccounting/Program.cs), where `DunningEngine` builds a `DunningCandidate` from data it already has in memory.

No repository call happens inside the specification; the specification just reasons over data it was handed.

### Option C — Use a Domain Service for Cross-Aggregate Orchestration

When a rule needs data that truly isn't available yet — it has to be queried fresh, possibly across aggregates — put that orchestration in a domain service, not in the specification.
The domain service owns the repository dependency and the async I/O; the specification stays pure and gets called once the service has assembled the data it needs.

```csharp
public sealed class DunningEligibilityService(IOpenItemRepository openItemRepository)
{
    public async Task<bool> IsEligibleAsync(LedgerAccount account, DateOnly asOf)
    {
        var openItems = await openItemRepository.GetOpenItemsAsync(account.AccountNumber);

        var itemSpec = new IsOpen().And(new IsNotDisputed()).And(new IsOverdue(asOf, graceDays: 21));

        return openItems.Any(itemSpec.IsSatisfiedBy);
    }
}
```

The repository dependency and the `await` live in the service.
`IsSatisfiedBy` never sees either.

## Summary

| Approach | Domain Purity | Testability | Performance | When to Use |
|---|---|---|---|---|
| Repository injected into specification | Poor — I/O hidden inside a boolean check | Hard — every test needs a repository mock | Risky — N+1 queries, sync-over-async | Avoid; pragmatic only in throwaway/legacy code |
| Pass data as constructor/method parameters | High | Easy — no mocks needed | Predictable — caller controls when I/O happens | Default choice when a rule needs one or two extra facts |
| Pre-fetch into a context object, specify against that | High | Easy — construct the record, no mocks | Predictable — one fetch, reused across specs | Rules spanning multiple aggregates, as in [`DunningCandidate`](../examples/OpenItemAccounting/Models/DunningCandidate.cs) |
| Domain service orchestrates repository + specifications | High | Clean — mock the repository once, at the service boundary | Controlled — one query, explicit `async` | Cross-aggregate rules where the data genuinely isn't loaded yet |

The rule of thumb: if a "check" wants to reach out and fetch something, that's a sign the fetching belongs one layer up — in the application service or a domain service — not inside the specification.
The specification's job is to decide, not to fetch.

See [How to use Specifications](How-to-use-Specifications.md) for the related question of how specifications should read aggregate state without breaking encapsulation, and the [main README](../readme.md#specifications-and-ddd-aggregates) for this repository's overall stance on specifications and aggregates.
