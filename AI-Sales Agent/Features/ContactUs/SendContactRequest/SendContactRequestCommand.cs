using MediatR;

namespace AI_Sales_Agent.Features.ContactUs.SendContactRequest;

public class SendContactRequestCommand : IRequest<bool>
{
    public string Email { get; set; } = string.Empty;

    public string StoreName { get; set; } = string.Empty;

    public string StoreDescription { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string ContactPreference { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
}