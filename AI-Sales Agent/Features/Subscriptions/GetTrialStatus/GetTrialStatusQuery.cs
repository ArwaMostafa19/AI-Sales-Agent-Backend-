using MediatR;

namespace AI_Sales_Agent.Features.Subscriptions.GetTrialStatus;

public class GetTrialStatusQuery : IRequest<GetTrialStatusResponse>
{
    public Guid UserId { get; set; }
}

public class GetTrialStatusResponse
{
    public bool IsInTrial { get; set; }

    public int RemainingDays { get; set; }

    public DateTime? TrialEndDate { get; set; }

    public string Status { get; set; } = string.Empty;
}