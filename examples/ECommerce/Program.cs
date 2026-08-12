using Phauthentic.Specification.Examples.ECommerce.Models;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Campaigns;

namespace Phauthentic.Specification.Examples.ECommerce;

/// <summary>
/// E-commerce Promotional Eligibility System Demo.
///
/// This demonstrates complex business rules using the Specification Pattern
/// with nested and composite specifications.
/// </summary>
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        RunDemo();
    }

    private static void RunDemo()
    {
        Console.WriteLine("=== E-COMMERCE PROMOTIONAL ELIGIBILITY SYSTEM ===\n");

        var products = CreateSampleProducts();
        var customers = CreateSampleCustomers();
        var orders = CreateSampleOrders(customers, products);

        var engine = new PromotionEngine();
        engine.AddPromotion(new Promotion(
            "black-friday-vip",
            "Black Friday VIP Campaign",
            "25% off for VIP customers during Black Friday",
            25,
            new BlackFridayVipCampaign()));
        engine.AddPromotion(new Promotion(
            "loyalty-reward",
            "Loyalty Reward Campaign",
            "15% off for loyal customers with rewards points",
            15,
            new LoyaltyRewardCampaign()));
        engine.AddPromotion(new Promotion(
            "flash-sale",
            "Flash Sale Campaign",
            "20% off during limited time windows",
            20,
            new FlashSaleCampaign()));
        engine.AddPromotion(new Promotion(
            "first-purchase",
            "First Purchase Discount",
            "10% off for new customers on first order",
            10,
            new FirstPurchaseCampaign()));

        var promotionNames = new (string Id, string Name)[]
        {
            ("black-friday-vip", "Black Friday VIP Campaign (25% off)"),
            ("loyalty-reward", "Loyalty Reward Campaign (15% off)"),
            ("flash-sale", "Flash Sale Campaign (20% off)"),
            ("first-purchase", "First Purchase Discount (10% off)"),
        };

        for (var i = 0; i < orders.Count; i++)
        {
            var order = orders[i];

            Console.WriteLine($"Testing Order #{i + 1}: {FormatOrderDetails(order)}\n");

            var eligiblePromotions = engine.GetEligiblePromotions(order);
            var bestPromotion = engine.GetBestPromotion(order);
            var eligibleIds = eligiblePromotions.Select(p => p.Id).ToHashSet();

            Console.WriteLine("Checking Promotions:");

            foreach (var (id, name) in promotionNames)
            {
                var symbol = eligibleIds.Contains(id) ? "✓" : "✗";
                Console.WriteLine($"  {symbol} {name}");

                if (!eligibleIds.Contains(id))
                {
                    var reason = GetIneligibilityReason(order, id);
                    if (reason is not null)
                    {
                        Console.WriteLine($"    ✗ {reason}");
                    }
                }
            }

            Console.WriteLine($"\nTotal Applicable Discounts: {eligiblePromotions.Count}");

            if (bestPromotion is not null)
            {
                var discount = bestPromotion.CalculateDiscount(order.TotalAmount);
                var finalPrice = bestPromotion.CalculateFinalPrice(order.TotalAmount);
                Console.WriteLine($"Best Discount: {bestPromotion.DiscountPercentage}% ({bestPromotion.Name})");
                Console.WriteLine($"Final Price: ${finalPrice:F2} (saved ${discount:F2})");
            }
            else
            {
                Console.WriteLine("No promotions apply");
            }

            Console.WriteLine("\n---\n");
        }

        Console.WriteLine("=== SUMMARY ===\n");
        Console.WriteLine("This demo showcases complex business rules implemented using the Specification Pattern:\n");
        Console.WriteLine("- Black Friday VIP Campaign: Nested AND/OR logic with customer tier, order value, product categories, account age, and date ranges");
        Console.WriteLine("- Loyalty Reward Campaign: Customer points, account age, item count, and product type exclusions");
        Console.WriteLine("- Flash Sale Campaign: Time-sensitive promotions with customer usage restrictions");
        Console.WriteLine("- First Purchase Campaign: New customer validation with product exclusions\n");
        Console.WriteLine("Each campaign demonstrates how atomic specifications can be composed into complex business rules.");
    }

    private static List<Product> CreateSampleProducts() =>
    [
        new("1", "iPhone 15", "electronics", 999.99m, false, false, 10),
        new("2", "Samsung TV", "electronics", 799.99m, false, false, 5),
        new("3", "Designer Dress", "fashion", 299.99m, false, false, 20),
        new("4", "Running Shoes", "fashion", 149.99m, false, false, 15),
        new("5", "Coffee Maker", "home", 89.99m, false, false, 8),
        new("6", "Novel Book", "books", 19.99m, false, false, 50),
        new("7", "Clearance Shirt", "fashion", 29.99m, true, false, 12),
        new("8", "Digital Game", "books", 49.99m, false, true, 100),
        new("9", "Gift Card", "gift", 50.00m, false, false, 25),
    ];

    private static List<Customer> CreateSampleCustomers()
    {
        var now = new DateTimeOffset(2025, 11, 25, 13, 0, 0, TimeSpan.Zero); // During Black Friday.

        return
        [
            // VIP Gold customer - should qualify for Black Friday.
            new Customer("1", "John Doe", "john@example.com", "gold", 1500,
                now.AddDays(-245), null, true),

            // Loyal customer with high points - should qualify for loyalty rewards.
            new Customer("2", "Jane Smith", "jane@example.com", "silver", 1200,
                now.AddDays(-90), null, true),

            // New customer - should qualify for first purchase.
            new Customer("3", "Bob Wilson", "bob@example.com", "bronze", 0,
                now.AddDays(-15), null, false),

            // Customer who recently used flash sale - won't qualify for flash sale.
            new Customer("4", "Alice Brown", "alice@example.com", "silver", 800,
                now.AddDays(-60), now.AddDays(-3), true),

            // New VIP - account too young for Black Friday.
            new Customer("5", "Charlie Davis", "charlie@example.com", "platinum", 500,
                now.AddDays(-20), null, true),
        ];
    }

    private static List<Order> CreateSampleOrders(IReadOnlyList<Customer> customers, IReadOnlyList<Product> products)
    {
        var now = new DateTimeOffset(2025, 11, 25, 13, 0, 0, TimeSpan.Zero); // During Black Friday & flash sale.

        return
        [
            // Order 1: VIP Gold during Black Friday - should qualify for multiple promotions.
            new Order("1", customers[0],
                [products[0], products[2], products[3], products[5]], // iPhone, Dress, Shoes, Book
                999.99m + 299.99m + 149.99m + 19.99m, 4, now, false),

            // Order 2: Loyal customer - should qualify for loyalty rewards.
            new Order("2", customers[1],
                [products[1], products[4], products[5]], // TV, Coffee Maker, Book
                799.99m + 89.99m + 19.99m, 3, now, false),

            // Order 3: New customer first order - should qualify for first purchase discount.
            new Order("3", customers[2],
                [products[3], products[5]], // Shoes, Book
                149.99m + 19.99m, 2, now, true),

            // Order 4: Small order during flash sale - may not qualify.
            new Order("4", customers[3],
                [products[0]], // iPhone
                999.99m, 1, now, false),

            // Order 5: New VIP with clearance items - won't qualify for Black Friday.
            new Order("5", customers[4],
                [products[6], products[5]], // Clearance Shirt, Book
                29.99m + 19.99m, 2, now, true),

            // Order 6: Digital products - won't qualify for first purchase.
            new Order("6", customers[2],
                [products[7], products[8]], // Digital Game, Gift Card
                49.99m + 50.00m, 2, now.AddDays(1), false),
        ];
    }

    private static string FormatOrderDetails(Order order)
    {
        var categories = order.GetCategoryCounts();
        var categoryStr = string.Join(", ", categories.Select(kv => $"{kv.Key}: {kv.Value}"));

        return $"{order.Customer.Name} ({Capitalize(order.Customer.Tier)}, {order.Customer.LoyaltyPoints} points)\n" +
               $"- Order Total: ${order.TotalAmount:F2}\n" +
               $"- Items: {order.ItemCount} ({categoryStr})\n" +
               $"- Account Age: {order.Customer.GetAccountAgeInDays()} days\n" +
               $"- Order Date: {order.CreatedAt:yyyy-MM-dd HH:mm:ss}";
    }

    private static string Capitalize(string value) =>
        value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..];

    private static string? GetIneligibilityReason(Order order, string promotionId) => promotionId switch
    {
        "black-friday-vip" => true switch
        {
            _ when order.Customer.Tier is not ("gold" or "platinum") => "Customer is not VIP (Gold/Platinum)",
            _ when order.TotalAmount < 100 => "Order total below $100 minimum",
            _ when !order.ContainsCategory("electronics") && !order.ContainsCategory("fashion") =>
                "Order does not contain electronics or fashion products",
            _ when order.Customer.GetAccountAgeInDays() < 180 => "Account younger than 6 months",
            _ => "Not within Black Friday date range",
        },
        "loyalty-reward" => true switch
        {
            _ when order.Customer.LoyaltyPoints < 1000 => "Insufficient loyalty points (< 1000)",
            _ when order.Customer.GetAccountAgeInDays() < 30 => "Account too new (< 30 days)",
            _ when order.ItemCount < 3 => "Order has fewer than 3 items",
            _ when order.HasClearanceItems => "Order contains clearance products",
            _ => null,
        },
        "flash-sale" => true switch
        {
            _ when order.Customer.DaysSinceLastFlashSale() is { } days && days < 7 =>
                "Customer used flash sale recently",
            _ when !order.ContainsCategory("electronics") && !order.ContainsCategory("fashion") =>
                "Order does not contain eligible categories",
            _ => "Not within flash sale time window",
        },
        "first-purchase" => true switch
        {
            _ when order.Customer.GetAccountAgeInDays() >= 30 => "Customer account too old (>= 30 days)",
            _ when !order.IsFirstOrder => "Not customer's first order",
            _ when order.TotalAmount < 50 => "Order total below $50 minimum",
            _ when order.HasDigitalItems || order.HasGiftCards =>
                "Order contains excluded products (digital/gift cards)",
            _ => null,
        },
        _ => null,
    };
}

/// <summary>
/// Collects promotions and evaluates order eligibility against them.
/// </summary>
internal sealed class PromotionEngine
{
    private readonly List<Promotion> _promotions = [];

    public void AddPromotion(Promotion promotion) => _promotions.Add(promotion);

    public List<Promotion> GetEligiblePromotions(Order order) =>
        _promotions.Where(p => p.IsEligible(order)).ToList();

    public Promotion? GetBestPromotion(Order order)
    {
        var eligible = GetEligiblePromotions(order);

        return eligible.Count == 0
            ? null
            : eligible.OrderByDescending(p => p.DiscountPercentage).First();
    }
}
