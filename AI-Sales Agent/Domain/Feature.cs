namespace AI_Sales_Agent.Domain
{
    public class Feature : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool? Enabled { get; set; }

        public ICollection<PlanFeature> PlanFeatures { get; set; } = new List<PlanFeature>();
    }
}
