# How to Use Specifications

This document explains different approaches to using the Specification Pattern, particularly in the context of Domain-Driven Design (DDD) where aggregates should encapsulate their internal state.

## The Challenge: Specifications and Encapsulation

In strict DDD, aggregates should not expose their internal properties directly.
This raises the question: How can a specification check conditions on an aggregate without breaking encapsulation?

```csharp
// This violates encapsulation - directly accessing internal state
public override bool IsSatisfiedBy(Order candidate) => candidate.TotalAmount >= _minimumValue;
```

There are several approaches to solve this, each with its own trade-offs.

---

## Approach 1: Aggregate Exposes Query Methods

Instead of exposing properties, the aggregate provides behavior-focused query methods.
The specification delegates the actual check to the aggregate.

**Pros:**

- Maintains encapsulation
- Business logic stays in the aggregate
- Specification remains composable

**Cons:**

- Aggregate interface grows with each new specification need
- May lead to many single-purpose query methods

### Example

```csharp
namespace App.Domain.Order;

/// <summary>
/// The Order aggregate - encapsulates all internal state.
/// </summary>
public sealed class Order
{
    private readonly decimal _totalAmount;
    private readonly DateTimeOffset _dueDate;
    private bool _noticeSent;
    private bool _inCollection;

    public Order(decimal totalAmount, DateTimeOffset dueDate)
    {
        _totalAmount = totalAmount;
        _dueDate = dueDate;
    }

    // Query methods - expose questions, not data
    public bool HasTotalAmountOfAtLeast(decimal minimum) => _totalAmount >= minimum;

    public bool IsOverdue(DateTimeOffset now) => _dueDate < now;

    public bool HasNoticeSent() => _noticeSent;

    public bool IsInCollection() => _inCollection;

    // Command methods
    public void MarkNoticeSent() => _noticeSent = true;

    public void SendToCollection() => _inCollection = true;
}
```

```csharp
namespace App.Domain.Order.Specifications;

using App.Domain.Order;
using Phauthentic.Specification;

/// <summary>
/// Specification that delegates to the aggregate's query method.
/// </summary>
public sealed class MinimumOrderValueSpecification(decimal minimumValue) : Specification<Order>
{
    public override bool IsSatisfiedBy(Order candidate) =>
        // Delegate to the aggregate - no direct property access
        candidate.HasTotalAmountOfAtLeast(minimumValue);
}
```

```csharp
namespace App.Domain.Order.Specifications;

using App.Domain.Order;
using Phauthentic.Specification;

/// <summary>
/// Specification for checking if an order is overdue.
/// </summary>
public sealed class OverdueSpecification(DateTimeOffset referenceDate) : Specification<Order>
{
    public override bool IsSatisfiedBy(Order candidate) => candidate.IsOverdue(referenceDate);
}
```

### Usage

```csharp
var now = DateTimeOffset.UtcNow;

var overdue = new OverdueSpecification(now);
var minimumValue = new MinimumOrderValueSpecification(100.00m);
var noticeSent = new NoticeSentSpecification();
var inCollection = new InCollectionSpecification();

// Compose specifications
var sendToCollection = overdue
    .And(noticeSent)
    .And(minimumValue)
    .AndNot(inCollection);

foreach (var order in orders)
{
    if (sendToCollection.IsSatisfiedBy(order))
    {
        order.SendToCollection();
    }
}
```

---

## Approach 2: Specifications on Read Models (CQRS)

In CQRS (Command Query Responsibility Segregation) architectures, specifications operate on read models or projections, not the aggregate itself.
The aggregate remains fully encapsulated for write operations.

**Pros:**

- Complete separation of read and write concerns
- Read models can be optimized for queries
- Aggregate stays fully encapsulated

**Cons:**

- Requires maintaining separate read models
- Eventual consistency between write and read sides
- More infrastructure complexity

### Example

```csharp
namespace App.ReadModel;

/// <summary>
/// Read model for orders - optimized for queries.
/// This is a projection, not the aggregate.
/// </summary>
public sealed record OrderReadModel(
    string OrderId,
    decimal TotalAmount,
    DateTimeOffset DueDate,
    bool NoticeSent,
    bool InCollection,
    string CustomerName,
    DateTimeOffset CreatedAt)
{
    public bool IsOverdue(DateTimeOffset now) => DueDate < now;
}
```

```csharp
namespace App.ReadModel.Specifications;

using App.ReadModel;
using Phauthentic.Specification;

/// <summary>
/// Specification operating on the read model.
/// </summary>
public sealed class MinimumOrderValueSpecification(decimal minimumValue) : Specification<OrderReadModel>
{
    public override bool IsSatisfiedBy(OrderReadModel candidate) =>
        // Direct property access is fine on read models
        candidate.TotalAmount >= minimumValue;
}
```

```csharp
namespace App.ReadModel.Specifications;

using App.ReadModel;
using Phauthentic.Specification;

/// <summary>
/// Specification for overdue orders on the read model.
/// </summary>
public sealed class OverdueSpecification(DateTimeOffset referenceDate) : Specification<OrderReadModel>
{
    public override bool IsSatisfiedBy(OrderReadModel candidate) => candidate.IsOverdue(referenceDate);
}
```

### Usage

```csharp
// Read side - uses read models from a projection/query service
var orderReadModels = orderQueryService.FindAllPendingOrders();

var now = DateTimeOffset.UtcNow;
var sendToCollection = new OverdueSpecification(now)
    .And(new NoticeSentSpecification())
    .AndNot(new InCollectionSpecification());

// Filter read models
var ordersToCollect = orderReadModels
    .Where(order => sendToCollection.IsSatisfiedBy(order));

// Write side - load aggregates only for the ones that need action
foreach (var orderReadModel in ordersToCollect)
{
    var order = orderRepository.GetById(orderReadModel.OrderId);
    order.SendToCollection();
    orderRepository.Save(order);
}
```

---

## Approach 3: Pragmatic Property Access

Many real-world implementations take a pragmatic stance: specifications are used for query/filtering logic where exposing read-only properties is acceptable.
The aggregate's invariants and mutation logic remain protected, but query-related properties can be exposed via public getters or immutable records.

**Pros:**

- Simple and straightforward
- Works well for filtering/querying use cases
- Full composability of specifications

**Cons:**

- Some encapsulation is sacrificed
- Requires discipline to not misuse exposed properties
- Purists may object

### Example

```csharp
namespace App.Domain.Order;

/// <summary>
/// Order aggregate with read-only property access for queries.
/// </summary>
public sealed class Order
{
    private bool _inCollection;
    private bool _noticeSent;

    public Order(string id, decimal totalAmount, DateTimeOffset dueDate)
    {
        Id = id;
        TotalAmount = totalAmount;
        DueDate = dueDate;
    }

    public string Id { get; }
    public decimal TotalAmount { get; }
    public DateTimeOffset DueDate { get; }

    // Getter for mutable state
    public bool IsNoticeSent() => _noticeSent;

    public bool IsInCollection() => _inCollection;

    // Commands remain protected
    public void MarkNoticeSent() => _noticeSent = true;

    public void SendToCollection() => _inCollection = true;
}
```

```csharp
namespace App.Domain.Order.Specifications;

using App.Domain.Order;
using Phauthentic.Specification;

/// <summary>
/// Specification using read-only properties.
/// </summary>
public sealed class MinimumOrderValueSpecification(decimal minimumValue) : Specification<Order>
{
    public override bool IsSatisfiedBy(Order candidate) =>
        // Access the read-only property directly
        candidate.TotalAmount >= minimumValue;
}
```

```csharp
namespace App.Domain.Order.Specifications;

using App.Domain.Order;
using Phauthentic.Specification;

/// <summary>
/// Specification for overdue orders.
/// </summary>
public sealed class OverdueSpecification(DateTimeOffset referenceDate) : Specification<Order>
{
    public override bool IsSatisfiedBy(Order candidate) => candidate.DueDate < referenceDate;
}
```

### Usage

```csharp
var now = DateTimeOffset.UtcNow;

var sendToCollection = new OverdueSpecification(now)
    .And(new NoticeSentSpecification())
    .AndNot(new InCollectionSpecification());

foreach (var order in orders)
{
    if (sendToCollection.IsSatisfiedBy(order))
    {
        order.SendToCollection();
    }
}
```

## Approach 4: Double Dispatch (The Purist Approach)

The aggregate remains completely "blind" to its properties from the outside.
Instead, the aggregate accepts the specification and "feeds" it the necessary data through a specific internal interface.

**Pros:**

- Zero leakage: Properties remain private and no getters are created.
- The aggregate controls exactly what data the specification is allowed to see.

**Cons:**

- Requires a custom interface for the specification.
- Slightly higher cognitive complexity.

```csharp
namespace App.Domain.Order;

/// <summary>
/// Interface that defines what data an Order spec is allowed to see.
/// </summary>
public interface IOrderData
{
    decimal GetTotalAmount();
    DateTimeOffset GetDueDate();
}

public interface IOrderSpecification
{
    bool IsSatisfiedByOrder(IOrderData order);
}

public sealed class Order : IOrderData
{
    private readonly decimal _totalAmount;
    private readonly DateTimeOffset _dueDate;

    // The aggregate "accepts" the spec and passes itself as the data provider
    public bool Satisfies(IOrderSpecification spec) => spec.IsSatisfiedByOrder(this);

    public decimal GetTotalAmount() => _totalAmount;
    public DateTimeOffset GetDueDate() => _dueDate;
}

// The specification now depends on the interface, not the aggregate
public sealed class MinimumOrderValueSpecification(decimal minimumValue) : IOrderSpecification
{
    public bool IsSatisfiedByOrder(IOrderData order) => order.GetTotalAmount() >= minimumValue;
}
```

---

## Choosing the Right Approach

| Approach | Best For | Avoid When |
|----------|----------|------------|
| **Query Methods** | Strict DDD, complex aggregates | Many specifications needed (interface bloat) |
| **Read Models (CQRS)** | Large systems, complex queries, event sourcing | Simple applications, tight deadlines |
| **Pragmatic Access** | Filtering, simple domains, rapid development | Strict encapsulation requirements |
| **Double Dispatch** | High encapsulation, shared logic | Overkill for simple logic; adds boilerplate |

## Combining Approaches

These approaches are not mutually exclusive.
A common pattern is to use read models for UI filtering (performance) and query methods or double dispatch for domain-critical business rules inside your services.
A common pattern is:

1. Use **Read Models** for complex queries and filtering (e.g., search, reports)
2. Use **Query Methods** for domain-critical business rules
3. Use **Internal Aggregate Methods** for invariant checks before state changes

```csharp
// Read side: filter candidates using specifications on read models
var candidates = orderQueryService.FindOverdueOrders();
var eligibleForCollection = new NoticeSentSpecification()
    .AndNot(new InCollectionSpecification());

var toProcess = candidates.Where(order => eligibleForCollection.IsSatisfiedBy(order));

// Write side: aggregate enforces its own invariants
foreach (var orderReadModel in toProcess)
{
    var order = orderRepository.GetById(orderReadModel.OrderId);

    // Aggregate validates internally before state change
    order.SendToCollection(); // May throw if invariants not met

    orderRepository.Save(order);
}
```
