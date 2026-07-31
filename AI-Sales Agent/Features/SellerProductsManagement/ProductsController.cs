using AI_Sales_Agent.Features.SellerProductsManagement.CreateProduct;
using AI_Sales_Agent.Features.SellerProductsManagement.DeleteProduct;
using AI_Sales_Agent.Features.SellerProductsManagement.GetAllProducts;
using AI_Sales_Agent.Features.SellerProductsManagement.GetProductById;
using AI_Sales_Agent.Features.SellerProductsManagement.UpdateMaxDiscount;
using AI_Sales_Agent.Features.SellerProductsManagement.UpdateProduct;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sales_Agent.Controllers;

[ApiController]
[Route("api/v1/products")]
[Authorize(Roles = Roles.Seller)] 
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var command = new CreateProductCommand(
            request.StoreId,
            request.OrganizationId,
            request.Title,
            request.Description,
            request.Price,
            request.Stock,
            request.MaxAllowedDiscount,
            request.Status ?? "draft",
            request.CategoryId,
            request.Vendor,
            request.Tags,
            request.ImageUrls
        );

        var productId = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetProductById),
            new { productId, storeId = request.StoreId },
            new { Id = productId, Message = "Product created successfully." }
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] string storeId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null)
    {
        var query = new GetAllProductsQuery(storeId, pageIndex, pageSize, search, status);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById([FromRoute] string productId, [FromQuery] string storeId)
    {
        var query = new GetProductByIdQuery(productId, storeId);
        var product = await _mediator.Send(query);

        if (product == null)
            return NotFound(new { Message = "Product not found." });

        return Ok(product);
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProduct(
        [FromRoute] string productId,
        [FromBody] UpdateProductRequest request)
    {
        var command = new UpdateProductCommand(
            productId,
            request.StoreId,
            request.Title,
            request.Description,
            request.Price,
            request.Stock,
            request.MaxAllowedDiscount,
            request.Status,
            request.CategoryId,
            request.Vendor,
            request.Tags,
            request.ImageUrls
        );

        var success = await _mediator.Send(command);
        if (!success)
            return NotFound(new { Message = "Product not found or failed to update." });

        return Ok(new { Message = "Product updated successfully." });
    }

    [HttpPatch("{productId}/max-discount")]
    public async Task<IActionResult> UpdateMaxDiscount(
        [FromRoute] string productId,
        [FromBody] UpdateMaxDiscountRequest request)
    {
        var command = new UpdateMaxDiscountCommand(productId, request.StoreId, request.MaxAllowedDiscount);
        var success = await _mediator.Send(command);

        if (!success)
            return NotFound(new { Message = "Product not found." });

        return Ok(new { Message = "Max discount updated successfully." });
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(
        [FromRoute] string productId,
        [FromQuery] string storeId,
        [FromQuery] bool softDelete = true)
    {
        var command = new DeleteProductCommand(productId, storeId, softDelete);
        var success = await _mediator.Send(command);

        if (!success)
            return NotFound(new { Message = "Product not found." });

        return Ok(new { Message = softDelete ? "Product archived successfully." : "Product permanently deleted." });
    }
}

#region --- Request DTOs ---

public record CreateProductRequest(
    string StoreId,
    string? OrganizationId = null,
    string Title = "",
    string? Description = null,
    double Price = 0,
    int Stock = 0,
    double MaxAllowedDiscount = 0,
    string? Status = "draft",
    string? CategoryId = null,
    string? Vendor = null,
    List<string>? Tags = null,
    List<string>? ImageUrls = null
);

public record UpdateProductRequest(
    string StoreId,
    string Title,
    string? Description,
    double Price,
    int Stock,
    double MaxAllowedDiscount,
    string Status,
    string? CategoryId = null,
    string? Vendor = null,
    List<string>? Tags = null,
    List<string>? ImageUrls = null
);

public record UpdateMaxDiscountRequest(
    string StoreId,
    double MaxAllowedDiscount
);

#endregion
