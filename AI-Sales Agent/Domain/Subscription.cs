namespace AI_Sales_Agent.Domain
{
    public class Subscription : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Status { get; set; } = string.Empty;

        public DateTime? RenewalDate { get; set; }

        //user
        public Guid UserId { get; set; }
        public User? User { get; set; }

        //plan
        public Guid PlanId { get; set; }
        public Plan? Plan { get; set; }
    }
}
