namespace AI_Sales_Agent.Domain
{
    public class Plan : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PlanName { get; set; } = string.Empty;

        public string PlanDescription { get; set; } = string.Empty;
        public string PlanStatus { get; set; } = string.Empty;

        public decimal PlanPrice { get; set; } = 0.0M;

        public long NumOfTokens { get; set; } = 0;

        public List<string> AiModels { get; set; } = new List<string>();

        //Subscription
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        //Plan Features
        public ICollection<PlanFeature> PlanFeatures { get; set; } = new List<PlanFeature>();
    }
}
