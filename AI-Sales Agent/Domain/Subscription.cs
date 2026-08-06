namespace AI_Sales_Agent.Domain
{
    public class Subscription : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Status { get; set; } = string.Empty;

        public DateTime? RenewalDate { get; set; }

        public bool IsTrial { get; set; }

        public DateTime? TrialStartDate { get; set; }

        public DateTime? TrialEndDate { get; set; }

        //stripe
        public string? StripeCustomerId { get; set; }

        public string? StripeSubscriptionId { get; set; }

        public string? StripePriceId { get; set; }

        //user
        public Guid UserId { get; set; }
        public User? User { get; set; }

        //plan
        public Guid PlanId { get; set; }
        public Plan? Plan { get; set; }

        //organization
        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }
}
