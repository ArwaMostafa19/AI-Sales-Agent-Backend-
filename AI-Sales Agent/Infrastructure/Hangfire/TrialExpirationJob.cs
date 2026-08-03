using AI_Sales_Agent.Services;

namespace AI_Sales_Agent.Infrastructure.Hangfire;

public class TrialExpirationJob
{
    private readonly ITrialService _trialService;

    public TrialExpirationJob(ITrialService trialService)
    {
        _trialService = trialService;
    }

    public async Task ExecuteAsync()
    {
        
        await _trialService.ExpireTrialsAsync();
    }
}