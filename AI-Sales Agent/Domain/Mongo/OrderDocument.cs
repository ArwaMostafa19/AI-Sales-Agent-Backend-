using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class OrderDocument : MongoBaseDocument
    {
        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("org_id")]
        public string OrgId { get; set; } = string.Empty;

        [BsonElement("external_id")]
        public string? ExternalId { get; set; }

        [BsonElement("customer_id")]
        public string? CustomerId { get; set; }

        [BsonElement("customer_email")]
        public string? CustomerEmail { get; set; }

        [BsonElement("line_items")]
        public List<LineItemModel> LineItems { get; set; } = new();

        [BsonElement("shipping_address")]
        public AddressModel? ShippingAddress { get; set; }

        [BsonElement("billing_address")]
        public AddressModel? BillingAddress { get; set; }

        [BsonElement("subtotal_price")]
        public MoneyModel? SubtotalPrice { get; set; }

        [BsonElement("total_price")]
        public MoneyModel? TotalPrice { get; set; }

        [BsonElement("total_tax")]
        public MoneyModel? TotalTax { get; set; }

        [BsonElement("total_discount")]
        public MoneyModel? TotalDiscount { get; set; }

        [BsonElement("shipping_price")]
        public MoneyModel? ShippingPrice { get; set; }

        [BsonElement("financial_status")]
        public string FinancialStatus { get; set; } = "pending";

        [BsonElement("fulfillment_status")]
        public string? FulfillmentStatus { get; set; }

        [BsonElement("currency")]
        public string Currency { get; set; } = "USD";

        [BsonElement("notes")]
        public string? Notes { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonElement("cancelled_at")]
        public DateTime? CancelledAt { get; set; }

        [BsonElement("audit")]
        public AuditInfoModel Audit { get; set; } = new();

        [BsonElement("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
