namespace AI_Sales_Agent.Domain
{
    public class StoreIntegrations : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Provider { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } 
        public DateTime? TokenExpiresAt { get; set; } 
        public DateTime? LastSync { get; set; }
        public string? WebhookSecret { get; set; }

        //store
        public Guid StoreId { get; set; }
        public Store? Store { get; set; }
    }
}
