using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class MoneyModel
    {
        [BsonElement("amount")]
        public double Amount { get; set; }

        [BsonElement("currency")]
        public string Currency { get; set; } = "USD";
    }

    public class DimensionsModel
    {
        [BsonElement("length")]
        public double? Length { get; set; }

        [BsonElement("width")]
        public double? Width { get; set; }

        [BsonElement("height")]
        public double? Height { get; set; }

        [BsonElement("unit")]
        public string? Unit { get; set; }
    }

    public class ImageModel
    {
        [BsonElement("url")]
        public string Url { get; set; } = string.Empty;

        [BsonElement("alt_text")]
        public string? AltText { get; set; }

        [BsonElement("width")]
        public int? Width { get; set; }

        [BsonElement("height")]
        public int? Height { get; set; }

        [BsonElement("position")]
        public int Position { get; set; }
    }

    public class VariantModel
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("sku")]
        public string? Sku { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("price")]
        public MoneyModel Price { get; set; } = new();

        [BsonElement("compare_at_price")]
        public MoneyModel? CompareAtPrice { get; set; }

        [BsonElement("inventory_quantity")]
        public int InventoryQuantity { get; set; }

        [BsonElement("weight")]
        public double? Weight { get; set; }

        [BsonElement("dimensions")]
        public DimensionsModel? Dimensions { get; set; }
    }

    public class ProductOptionModel
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("values")]
        public List<string> Values { get; set; } = new();
    }

    public class SEOModel
    {
        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("url_slug")]
        public string? UrlSlug { get; set; }
    }

    public class AuditInfoModel
    {
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_by")]
        public string? UpdatedBy { get; set; }
    }

    public class AddressModel
    {
        [BsonElement("first_name")]
        public string? FirstName { get; set; }

        [BsonElement("last_name")]
        public string? LastName { get; set; }

        [BsonElement("line1")]
        public string? Line1 { get; set; }

        [BsonElement("line2")]
        public string? Line2 { get; set; }

        [BsonElement("city")]
        public string? City { get; set; }

        [BsonElement("state")]
        public string? State { get; set; }

        [BsonElement("zip")]
        public string? Zip { get; set; }

        [BsonElement("country")]
        public string? Country { get; set; }

        [BsonElement("phone")]
        public string? Phone { get; set; }
    }

    public class TaxLineModel
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("rate")]
        public double Rate { get; set; }

        [BsonElement("price")]
        public MoneyModel Price { get; set; } = new();
    }

    public class DiscountAllocationModel
    {
        [BsonElement("amount")]
        public MoneyModel Amount { get; set; } = new();

        [BsonElement("discount_application_index")]
        public int? DiscountApplicationIndex { get; set; }
    }

    public class LineItemModel
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("variant_id")]
        public string? VariantId { get; set; }

        [BsonElement("product_id")]
        public string? ProductId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("price")]
        public MoneyModel Price { get; set; } = new();

        [BsonElement("tax_lines")]
        public List<TaxLineModel> TaxLines { get; set; } = new();

        [BsonElement("discount_allocations")]
        public List<DiscountAllocationModel> DiscountAllocations { get; set; } = new();
    }

    public class PaginationModel
    {
        [BsonElement("page_param")]
        public string? PageParam { get; set; }

        [BsonElement("limit_param")]
        public string? LimitParam { get; set; }

        [BsonElement("default_limit")]
        public int? DefaultLimit { get; set; }

        [BsonElement("cursor_field")]
        public string? CursorField { get; set; }

        [BsonElement("total_field")]
        public string? TotalField { get; set; }

        [BsonElement("next_link_field")]
        public string? NextLinkField { get; set; }
    }
}
