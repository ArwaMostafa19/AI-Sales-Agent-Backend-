using MediatR;
using System.Net.Http;

namespace AI_Sales_Agent.Features.ContactUs.SendContactRequest;

public class SendContactRequestHandler
    : IRequestHandler<SendContactRequestCommand, bool>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public SendContactRequestHandler(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<bool> Handle(
        SendContactRequestCommand request,
        CancellationToken cancellationToken)
    {
        var botToken = _configuration["Telegram:BotToken"];
        var chatId = _configuration["Telegram:ChatId"];

        if (string.IsNullOrWhiteSpace(botToken))
            throw new InvalidOperationException(
                "Telegram bot token is not configured.");

        if (string.IsNullOrWhiteSpace(chatId))
            throw new InvalidOperationException(
                "Telegram chat ID is not configured.");
        Console.WriteLine($"Bot Token exists: {!string.IsNullOrWhiteSpace(botToken)}");
        Console.WriteLine($"Chat ID = {chatId}");
        var message =
            $"📩 New Development Request\n\n" +
            $"🏪 Store Name: {request.StoreName}\n" +
            $"Store description: {request.StoreDescription}\n" +
            $"📧 Email: {request.Email}\n" +
            $"📱 Phone: {request.PhoneNumber}\n" +
            $"📞 Contact Preference: {request.ContactPreference}\n" +
            $"💬 Message:{request.Message}\n" +
            $"Notes: {request.Notes}\n";

        var client = _httpClientFactory.CreateClient();

        var url =
            $"https://api.telegram.org/bot{botToken}/sendMessage";

        var content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["chat_id"] = chatId,
                ["text"] = message
            });

        var response = await client.PostAsync(url, content);

        var responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Telegram Status: {response.StatusCode}");
        Console.WriteLine($"Telegram Response: {responseBody}");

        if (!response.IsSuccessStatusCode)
        {
            throw new BadHttpRequestException(
                $"Telegram failed: {responseBody}");
        }

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            throw new HttpRequestException(
                $"Telegram request failed: {error}");
        }

        return true;
    }
}