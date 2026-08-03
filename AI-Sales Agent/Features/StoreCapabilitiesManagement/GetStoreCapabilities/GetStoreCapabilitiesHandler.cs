using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerStoreManagement.GetStoreCapabilities;

public class GetStoreCapabilitiesHandler : IRequestHandler<GetStoreCapabilitiesQuery, StoreCapabilitiesResponseDto?>
{
    private readonly MongoDbContext _context;

    public GetStoreCapabilitiesHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<StoreCapabilitiesResponseDto?> Handle(GetStoreCapabilitiesQuery request, CancellationToken cancellationToken)
    {
        string storeIdStr = request.StoreId.ToLower();

        // فلترة بواسطة الـ StoreId
        var filter = Builders<StoreCapabilitiesDocument>.Filter.Eq(s => s.StoreId, storeIdStr);

        var doc = await _context.StoreCapabilities
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        if (doc == null)
            return null;

        // تحويل الـ BsonDocument الخاص بالـ Capabilities إلى Dictionary عادي
        var capabilitiesDict = new Dictionary<string, bool>();

        if (doc.Capabilities != null)
        {
            foreach (var element in doc.Capabilities)
            {
                if (element.Value.IsBoolean)
                {
                    capabilitiesDict[element.Name] = element.Value.AsBoolean;
                }
            }
        }

        return new StoreCapabilitiesResponseDto(doc.StoreId, capabilitiesDict);
    }
}