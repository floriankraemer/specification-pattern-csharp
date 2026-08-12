# E-Commerce Promotional System Example

A comprehensive example demonstrating complex business rules using the Specification Pattern in C#.
This example shows how to implement nested and composite specifications for e-commerce promotional campaigns.

## Overview

This example implements a realistic e-commerce promotional eligibility system that validates whether customers, products, and orders qualify for various promotional campaigns.
It demonstrates the power of the Specification Pattern for implementing complex, nested business rules that can be easily composed, tested, and maintained.

## Business Scenario

The system implements four promotional campaigns with real-world business rules:

### 1. Black Friday VIP Campaign (25% off)
- Customer must be VIP (gold/platinum tier)
- Order total must be at least $100
- Must include products from electronics **OR** fashion categories
- Customer account must be older than 6 months
- Promotion valid only during November 20-30

### 2. Loyalty Reward Campaign (15% off)
- Customer has 1000+ loyalty points
- Not a new customer (account > 30 days)
- Order contains at least 3 items
- Excludes clearance products

### 3. Flash Sale Campaign (20% off)
- Time-sensitive (valid for specific hours, e.g., 12:00-14:00)
- Limited to specific product categories (electronics, fashion)
- Customer must not have used a flash sale in the last 7 days

### 4. First Purchase Discount (10% off)
- New customer (account < 30 days)
- First order ever
- Minimum order value $50
- Excludes gift cards and digital products

## Running the Example

### Prerequisites
- .NET 8 SDK, or Docker (see the [repository README](../../readme.md) for the Docker workflow)

### Running locally

```bash
dotnet run --project examples/ECommerce/ECommerce.csproj
```

### Running via Docker

```bash
make up
make run-example
```

### Sample Output

```
=== E-COMMERCE PROMOTIONAL ELIGIBILITY SYSTEM ===

Testing Order #1: John Doe (Gold, 1500 points)
- Order Total: $1469.96
- Items: 4 (electronics: 1, fashion: 2, books: 1)
- Account Age: 245 days
- Order Date: 2025-11-25 13:00:00

Checking Promotions:
  ✓ Black Friday VIP Campaign (25% off)
  ✓ Loyalty Reward Campaign (15% off)
  ✓ Flash Sale Campaign (20% off)
  ✗ First Purchase Discount (10% off)
    ✗ Customer account too old (>= 30 days)

Total Applicable Discounts: 3
Best Discount: 25% (Black Friday VIP Campaign)
Final Price: $1102.47 (saved $367.49)
```

> **Note:** the sample data anchors customer/order dates to a fixed point in time (November 25, 2025), but `Customer.GetAccountAgeInDays()`/`DaysSinceLastFlashSale()` compute against the real wall-clock `DateTimeOffset.UtcNow` — exactly like the PHP original.
> This means the "Account Age" printed above drifts further from the `245 days` shown in this README the longer it's been since that anchor date, and the exact promotions each order qualifies for can change over time (e.g. accounts eventually age past the "new customer" thresholds).
> This is a faithful port of a quirk already present in the original PHP demo, not something introduced by this port — see the [repository README](../../readme.md#deviations-from-the-php-original) for the deviations that *were* made deliberately.

## Architecture

### Domain Models
- **`Customer`**: Represents customers with tier, loyalty points, account age, etc.
- **`Product`**: Represents products with category, price, clearance status, etc.
- **`Order`**: Represents orders with customer, items, totals, and metadata
- **`Promotion`**: Represents promotional campaigns with discount rules

### Specification Hierarchy

```
├── Customer/
│   ├── IsVipCustomerSpecification
│   ├── HasLoyaltyPointsSpecification
│   ├── AccountAgeSpecification
│   ├── IsNewCustomerSpecification
│   └── HasNotUsedFlashSaleRecentlySpecification
├── Product/
│   ├── ProductCategorySpecification
│   ├── IsNotClearanceSpecification
│   ├── IsNotDigitalProductSpecification
│   └── PriceRangeSpecification
├── Order/
│   ├── MinimumOrderValueSpecification
│   ├── MinimumItemCountSpecification
│   ├── ContainsProductCategorySpecification
│   └── IsFirstOrderSpecification
├── Time/
│   ├── DateRangeSpecification
│   └── TimeRangeSpecification
└── Campaigns/
    ├── BlackFridayVipCampaign (Composite)
    ├── LoyaltyRewardCampaign (Composite)
    ├── FlashSaleCampaign (Composite)
    └── FirstPurchaseCampaign (Composite)
```

The `Customer/*` and `Order/*` specifications are all typed as `Specification<Order>` (rather than accepting either a `Customer` or an `Order` candidate, as the PHP original does via a runtime `instanceof` check) because every campaign in this demo composes them against an `Order`.
This is the one place the C# generic typing narrows behavior compared to the PHP original — see the [repository README](../../readme.md#deviations-from-the-php-original) for details.

### Complex Specification Composition

The Black Friday VIP Campaign demonstrates nested AND/OR logic:

```csharp
// Black Friday VIP Campaign
var blackFridaySpec = new IsVipCustomerSpecification(["gold", "platinum"])
    .And(new MinimumOrderValueSpecification(100m))
    .And(
        new ContainsProductCategorySpecification("electronics")
            .Or(new ContainsProductCategorySpecification("fashion")))
    .And(new AccountAgeSpecification(180, AccountAgeComparison.Minimum));
    // The date range check runs separately against candidate.CreatedAt, since
    // DateRangeSpecification is a Specification<DateTimeOffset>, not Specification<Order>.
```

## Test Scenarios

The example includes diverse test cases:

1. **VIP customer during Black Friday** - Multiple promotions apply
2. **Loyal customer with high points** - Loyalty rewards
3. **New customer with first order** - First purchase discount
4. **Customer during flash sale window** - Time-sensitive rules
5. **Edge cases** - Customers who almost qualify but miss criteria

## Key Benefits Demonstrated

### 1. Reusability
Individual specifications are used across multiple campaigns:
- `AccountAgeSpecification` used in Black Friday and Loyalty campaigns
- `ContainsProductCategorySpecification` used in multiple promotions

### 2. Maintainability
Business rules are centralized and easy to modify:
```csharp
// Change VIP tiers across all campaigns
new IsVipCustomerSpecification(["gold", "platinum", "diamond"]);
```

### 3. Testability
Each specification can be unit tested independently:
```csharp
var spec = new IsVipCustomerSpecification(["gold"]);
Assert.True(spec.IsSatisfiedBy(vipCustomerOrder));
Assert.False(spec.IsSatisfiedBy(regularCustomerOrder));
```

### 4. Composability
Complex rules built from simple, atomic specifications:
```csharp
var complexRule = customerRule.And(orderRule).And(timeRule);
```

### 5. Readability
Business rules expressed in domain language:
```csharp
var loyaltyCampaign = hasPoints.And(notNewCustomer).And(hasMinItems);
```

### 6. Flexibility
New campaigns created by combining existing specifications:
```csharp
var newCampaign = existingSpec1.Or(existingSpec2).And(newSpec);
```

## Extending the System

### Adding a New Campaign
1. Create atomic specifications if needed
2. Compose them into a campaign specification
3. Add to the promotion engine

```csharp
public sealed class HolidaySeasonCampaign : Specification<Order>
{
    private static readonly DateRangeSpecification DateRangeSpec = new(
        new DateOnly(2025, 12, 1), new DateOnly(2025, 12, 31));

    private readonly ISpecification<Order> _orderSpecification =
        new MinimumOrderValueSpecification(75m)
            .And(new IsNewsletterSubscriberSpecification());

    public override bool IsSatisfiedBy(Order candidate) =>
        _orderSpecification.IsSatisfiedBy(candidate) && DateRangeSpec.IsSatisfiedBy(candidate.CreatedAt);
}
```

### Adding New Specifications
```csharp
public sealed class IsNewsletterSubscriberSpecification : Specification<Order>
{
    public override bool IsSatisfiedBy(Order candidate) => candidate.Customer.IsNewsletterSubscriber;
}
```

## Business Rule Examples

| Campaign | Customer Criteria | Order Criteria | Product Criteria | Time Criteria |
|----------|------------------|----------------|------------------|---------------|
| Black Friday VIP | VIP tier, 6+ months | ≥$100 | Electronics OR Fashion | Nov 20-30 |
| Loyalty Rewards | 1000+ points, 30+ days | ≥3 items | No clearance | Always |
| Flash Sale | Not used in 7 days | Any | Electronics/Fashion | 12:00-14:00 |
| First Purchase | <30 days, first order | ≥$50 | No digital/gift cards | Always |

## Real-World Applications

This pattern is ideal for:
- **E-commerce**: Product recommendations, pricing rules, shipping eligibility
- **Financial Services**: Loan approvals, credit scoring, fraud detection
- **Insurance**: Policy eligibility, risk assessment, claims processing
- **Healthcare**: Treatment eligibility, appointment scheduling, billing rules
- **Travel**: Booking rules, loyalty programs, discount eligibility

## Running Tests

```bash
dotnet test
```

Or via Docker:

```bash
make test
```

## Performance Considerations

- Specifications are evaluated lazily
- Composite specifications short-circuit when possible (`And`/`Or` use `&&`/`||`, and `CompositeSpecification<T>.IsSatisfiedBy` uses `Enumerable.All`, which short-circuits)
- Results can be cached for expensive operations
- Database queries can be optimized based on specification requirements

## Contributing

When adding new specifications:
1. Follow the existing naming conventions
2. Add comprehensive unit tests
3. Update this README with business rules
4. Ensure composability with existing specifications

---

This example demonstrates how the Specification Pattern enables clean, maintainable, and testable implementations of complex business rules in domain-driven design.
