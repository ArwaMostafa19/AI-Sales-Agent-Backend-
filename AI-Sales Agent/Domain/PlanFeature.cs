namespace AI_Sales_Agent.Domain
{
    public class PlanFeature : BaseEntity
    {
        public Guid PlanId { get; set; }
        public Plan Plan { get; set; } = null!;

        public Guid FeatureId { get; set; }
        public Feature Feature { get; set; } = null!;
    }
}
