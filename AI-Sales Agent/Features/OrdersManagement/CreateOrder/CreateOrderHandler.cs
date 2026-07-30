using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.OrdersManagement.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, string>
{
    private readonly MongoDbContext _context;

    public CreateOrderHandler(MongoDbContext context)
    {
        _context = context;
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
                        // جلب الـ Variant المطلوب أو افتراض الأول
                        var targetVariant = product.Variants.FirstOrDefault(v => v.Id == item.VariantId)
                                           ?? product.Variants.FirstOrDefault();

                        // 👈 هنا التصحيح: بنجيب الـ Amount الرقمي جوه الـ Price
                        double unitPrice = targetVariant?.Price?.Amount ?? 0.0;
                        item.Price = new MoneyModel { Amount = unitPrice, Currency = request.Currency };

                        calculatedSubtotal += unitPrice * item.Quantity;

                        // 📦 خصم الـ Stock من الـ Variant المحدد
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

                    // تجميع الخصومات المبعوثة من الـ AI إن وجدت
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
            ShippingAddress = request.ShippingAddress,
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

        // 3. 💰 تحديث الـ Total Revenue بالصافي الحقيقي
        if (request.FinancialStatus.Equals("paid", StringComparison.OrdinalIgnoreCase))
        {
            var insightsFilter = Builders<DashboardInsightDocument>.Filter.Eq(d => d.StoreId, request.StoreId);

            var updateInsights = Builders<DashboardInsightDocument>.Update
                .Inc("total_revenue", finalTotalPrice)
                .SetOnInsert("recommendations", new List<string>())
                .Set(d => d.CalculatedAt, DateTime.UtcNow);

            await _context.DashboardInsights.UpdateOneAsync(
                insightsFilter,
                updateInsights,
                new UpdateOptions { IsUpsert = true },
                cancellationToken);
        }

        return order.Id;
    }
}