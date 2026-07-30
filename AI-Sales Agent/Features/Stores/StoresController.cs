using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Features.Stores.CreateStore;
using AI_Sales_Agent.Features.Stores.DeleteStore;
using AI_Sales_Agent.Features.Stores.GetStore;
using AI_Sales_Agent.Features.Stores.GetStores;
using AI_Sales_Agent.Features.Stores.UpdateStore;
using AI_Sales_Agent.Features.Stores.UpdateStoreSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AI_Sales_Agent.Infrastructure.Auth;

namespace AI_Sales_Agent.Features.Stores
{
    [ApiController]
    [Authorize(Roles = Roles.Seller)]
    [Route("api/stores")]
    public class StoresController : ControllerBase
    {
        private readonly ISender _sender;

        public StoresController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<StoreResponse>>> GetStores(CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetStoresQuery(), cancellationToken));
        }

        [HttpGet("{storeId:guid}")]
        public async Task<ActionResult<StoreResponse>> GetStore(Guid storeId, CancellationToken cancellationToken)
        {
            var store = await _sender.Send(new GetStoreQuery(storeId), cancellationToken);
            return store is null ? NotFound() : Ok(store);
        }

        [HttpPost]
        public async Task<ActionResult<StoreResponse>> CreateStore(CreateStoreCommand command, CancellationToken cancellationToken)
        {
            var store = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetStore), new { storeId = store.Id }, store);
        }

        [HttpPut("{storeId:guid}")]
        public async Task<ActionResult<StoreResponse>> UpdateStore(
            Guid storeId,
            UpdateStoreRequest request,
            CancellationToken cancellationToken)
        {
            var store = await _sender.Send(
                new UpdateStoreCommand(
                    storeId,
                    request.Name,
                    request.Description,
                    request.Platform,
                    request.ShopDomain,
                    request.Status),
                cancellationToken);

            return store is null ? NotFound() : Ok(store);
        }

        [HttpPut("{storeId:guid}/settings")]
        public async Task<ActionResult<StoreResponse>> UpdateStoreSettings(
            Guid storeId,
            UpdateStoreSettingsRequest request,
            CancellationToken cancellationToken)
        {
            var store = await _sender.Send(
                new UpdateStoreSettingsCommand(
                    storeId,
                    request.Currency,
                    request.Language,
                    request.Timezone),
                cancellationToken);

            return store is null ? NotFound() : Ok(store);
        }

        [HttpDelete("{storeId:guid}")]
        public async Task<IActionResult> DeleteStore(Guid storeId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeleteStoreCommand(storeId), cancellationToken);
            return result.Succeeded ? Ok(result) : NotFound(result);
        }
    }

    public record UpdateStoreRequest(
        string Name,
        string Description,
        string Platform,
        string ShopDomain,
        string Status);

    public record UpdateStoreSettingsRequest(
        string Currency,
        string Language,
        string Timezone);
}
