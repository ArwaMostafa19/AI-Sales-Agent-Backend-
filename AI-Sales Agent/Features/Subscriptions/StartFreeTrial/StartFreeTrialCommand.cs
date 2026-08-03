using MediatR;
using System.Text.Json.Serialization;

namespace AI_Sales_Agent.Features.Subscriptions.StartFreeTrial;

public class StartFreeTrialCommand
    : IRequest<StartFreeTrialResponse>
{
    public Guid PlanId { get; set; }

    [JsonIgnore]
    public Guid UserId { get; set; }
}

public class StartFreeTrialResponse
{
    public Guid SubscriptionId { get; set; }

    public DateTime TrialEndDate { get; set; }

    public int TrialDays { get; set; }

    public string Message { get; set; } = string.Empty;
}