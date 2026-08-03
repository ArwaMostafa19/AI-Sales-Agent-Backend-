using AI_Sales_Agent.Data;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Services;

public class TrialService : ITrialService
{
    private readonly ApplicationDbContext _context;

    public TrialService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ExpireTrialsAsync()
    {
        //------------------------------------------------------------
        // التعديل:
        // نجيب كل الـ Trials اللي انتهت ولسه Status = Trial
        //------------------------------------------------------------

        var expiredTrials = await _context.Subscriptions
            .Where(s =>
                s.DeletedAt == null &&
                s.IsTrial &&
                s.Status == "Trial" &&
                s.TrialEndDate <= DateTime.UtcNow)
            .ToListAsync();

        //------------------------------------------------------------
        // التعديل:
        // لو مفيش ولا Trial انتهت
        //------------------------------------------------------------

        if (!expiredTrials.Any())
            return;

        //------------------------------------------------------------
        // التعديل:
        // نحولها Cancelled
        //------------------------------------------------------------

        foreach (var subscription in expiredTrials)
        {
            subscription.Status = "Cancelled";

            subscription.IsTrial = false;

            subscription.UpdatedAt = DateTime.UtcNow;
        }

        //------------------------------------------------------------
        // التعديل:
        // نحفظ التغييرات مرة واحدة
        //------------------------------------------------------------

        await _context.SaveChangesAsync();
    }
}