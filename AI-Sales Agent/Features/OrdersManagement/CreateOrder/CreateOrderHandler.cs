using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Features.DashboardManagement.GetRevenueGrowth;
using AI_Sales_Agent.Infrastructure.Mongo;
using AI_Sales_Agent.Services;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.OrdersManagement.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, string>
{
    private readonly MongoDbContext _context;
    private readonly IDashboardNotifier _notifier; // 👈 1. ضفنا الـ Notifier هنا

    public CreateOrderHandler(MongoDbContext context, IDashboardNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task<string> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        double calculatedSubtotal = 0.0;
        double totalDiscounts = 0.0;

        // 1. 🔍 جلب الأسعار الحقيقية من DB وخصم الـ Stock من الـ Variant الصحيح
        if (request.LineItems != null && request.LineItems.Any())
        {
            foreach (var item in request.LineItems)
            {
                if (!string.IsNullOrEmpty(item.ProductId) && item.Quantity > 0)
                {
                    var productFilter = Builders<ProductDocument>.Filter.And(
                        Builders<ProductDocument>.Filter.Eq(p => p.Id, item.ProductId),
                        Builders<ProductDocument>.Filter.Eq(p => p.StoreId, request.StoreId)
                    );

                    var product = await _context.Products.Find(productFilter).FirstOrDefaultAsync(cancellationToken);

                    if (product != null)
                    {
                        var targetVariant = product.Variants.FirstOrDefault(v => v.Id == item.VariantId)
                                           ?? product.Variants.FirstOrDefault();

                        double unitPrice = targetVariant?.Price?.Amount ?? 0.0;
                        item.Price = new MoneyModel { Amount = unitPrice, Currency = request.Currency };

                        calculatedSubtotal += unitPrice * item.Quantity;

                        if (targetVariant != null)
                        {
                            var stockFilter = Builders<ProductDocument>.Filter.And(
                                Builders<ProductDocument>.Filter.Eq(p => p.Id, item.ProductId),
                                Builders<ProductDocument>.Filter.ElemMatch(p => p.Variants, v => v.Id == targetVariant.Id)
                            );

                            var updateStock = Builders<ProductDocument>.Update
                                .Inc("variants.$.inventory_quantity", -item.Quantity)
                                .Set(p => p.Audit.UpdatedAt, DateTime.UtcNow);

                            await _context.Products.UpdateOneAsync(stockFilter, updateStock, cancellationToken: cancellationToken);
                        }
                    }

                    if (item.DiscountAllocations != null && item.DiscountAllocations.Any())
                    {
                        totalDiscounts += item.DiscountAllocations
                            .Where(d => d.Amount != null)
                            .Sum(d => d.Amount.Amount);
                    }
                }
            }
        }

        // حساب الصافي النهائي
        double finalTotalPrice = Math.Max(0, calculatedSubtotal - totalDiscounts);

        // 2. 📝 حفظ الأوردر بالبيانات المحسوبة
        var order = new OrderDocument
        {
            StoreId = request.StoreId,
            OrgId = request.OrgId,
            CustomerId = request.CustomerId,
            CustomerEmail = request.CustomerEmail,
            LineItems = request.LineItems,
            ShippingAddress = request.ShippingAddress!,
            SubtotalPrice = new MoneyModel { Amount = calculatedSubtotal, Currency = request.Currency },
            TotalDiscount = new MoneyModel { Amount = totalDiscounts, Currency = request.Currency },
            TotalPrice = new MoneyModel { Amount = finalTotalPrice, Currency = request.Currency },
            Currency = request.Currency,
            FinancialStatus = request.FinancialStatus,
            Audit = new AuditInfoModel
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await _context.Orders.InsertOneAsync(order, cancellationToken: cancellationToken);

        // 3. 💰 تحديث الـ Total Revenue + Growth وترشيق الإشعارات لحظياً عبر SignalR
        // 3. 💰 تحديث الـ Total Revenue + Growth وترشيق الإشعارات لحظياً عبر SignalR
        if (request.FinancialStatus.Equals("paid", StringComparison.OrdinalIgnoreCase))
        {
            string storeIdStr = request.StoreId.ToLower();
            var insightsFilter = Builders<DashboardInsightDocument>.Filter.Eq(d => d.StoreId, storeIdStr);

            var existingInsight = await _context.DashboardInsights.Find(insightsFilter).FirstOrDefaultAsync(cancellationToken);

            // 🟢 الـ oldTotalRevenue يقرأ من الداتابيز (ثابت زي ما هو)
            double oldTotalRevenue = existingInsight?.OldTotalRevenue ?? 0.0;

            // 🟢 الـ totalRevenue الحالي يزيد بقيمة الأوردر الجديد
            double currentTotalRevenue = existingInsight?.TotalRevenue ?? 0.0;
            double newTotalRevenue = currentTotalRevenue + finalTotalPrice;

            // 🟢 حساب نسبة النمو الجديدة بين الإيراد الجديد والـ old الثابت
            double growthPercentage = CalculateGrowthPercentage(oldTotalRevenue, newTotalRevenue);

            string status = growthPercentage switch
            {
                > 0 => "Positive",
                < 0 => "Negative",
                _ => "Neutral"
            };

            // 🟢 تحديث MongoDB (بنحدث total_revenue و growth_percentage فقط، وبنسيب old_total_revenue زي ما هو)
            var updateInsights = Builders<DashboardInsightDocument>.Update
                .Set(d => d.StoreId, storeIdStr)
                .Set(d => d.TotalRevenue, newTotalRevenue)
                .Set(d => d.GrowthPercentage, growthPercentage)
                .Set(d => d.CalculatedAt, DateTime.UtcNow)
                .SetOnInsert(d => d.Recommendations, new List<string>());

            await _context.DashboardInsights.UpdateOneAsync(
                insightsFilter,
                updateInsights,
                new UpdateOptions { IsUpsert = true },
                cancellationToken);

            // 🚀 4. إرسال الإشعارات للفرونت فوراً عبر SignalR
            var growthResponse = new RevenueGrowthResponseDto(growthPercentage, status);

            await _notifier.NotifyTotalRevenueUpdatedAsync(request.StoreId, newTotalRevenue);
            await _notifier.NotifyRevenueGrowthUpdatedAsync(request.StoreId, growthResponse);
        }

        return order.Id;
    }

    private static double CalculateGrowthPercentage(double oldRevenue, double newRevenue)
    {
        if (oldRevenue == 0 && newRevenue == 0) return 0.0;
        if (oldRevenue == 0) return 100.0;

        double percentage = (newRevenue - oldRevenue) / oldRevenue * 100;
        return Math.Round(percentage, 2);
    }
}