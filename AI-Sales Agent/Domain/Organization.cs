using System;
using System.Collections.Generic;

namespace AI_Sales_Agent.Domain
{
    public class Organization : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SubscriptionPlan { get; set; }
        public string? SubscriptionStatus { get; set; }
        public string? Timezone { get; set; }
        public string? Country { get; set; }

        // 1:1 Relationship with User
        public Guid? UserId { get; set; }
        public User? User { get; set; }

        // 1:N Stores
        public ICollection<Store> Stores { get; set; } = new List<Store>();

        // 1:1 Subscription
        public Subscription? Subscription { get; set; }

        // 1:N Features
        public ICollection<Feature> Features { get; set; } = new List<Feature>();
    }
}
