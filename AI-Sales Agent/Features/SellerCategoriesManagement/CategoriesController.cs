using AI_Sales_Agent.Features.SellerCategoriesManagement.CreateCategory;
using AI_Sales_Agent.Features.SellerCategoriesManagement.DeleteCategory;
using AI_Sales_Agent.Features.SellerCategoriesManagement.GetAllCategories;
using AI_Sales_Agent.Features.SellerCategoriesManagement.GetCategoryById;
using AI_Sales_Agent.Features.SellerCategoriesManagement.UpdateCategory;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement;

[ApiController]
[Route("api/v1/categories")]
[Authorize(Roles = Roles.Seller)]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // 1. Create Category
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var categoryId = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetCategoryById),
            new { categoryId, storeId = command.StoreId },
            new { Id = categoryId, Message = "Category created successfully." }
        );
    }

    // 2. Get All Categories
    [HttpGet]
    public async Task<IActionResult> GetAllCategories([FromQuery] string storeId, [FromQuery] string? search = null)
    {
        var query = new GetAllCategoriesQuery(storeId, search);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // 3. Get Category By ID
    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] string categoryId, [FromQuery] string storeId)
    {
        var query = new GetCategoryByIdQuery(categoryId, storeId);
        var category = await _mediator.Send(query);

        if (category == null)
            return NotFound(new { Message = "Category not found." });

        return Ok(category);
    }

    // 4. Update Category
    [HttpPut("{categoryId}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] string categoryId, [FromBody] UpdateCategoryDto body)
    {
        var command = new UpdateCategoryCommand(
            categoryId,
            body.StoreId,
            body.Name,
            body.Description,
            body.ParentId,
            body.ImageUrl,
            body.SortOrder
        );

        var success = await _mediator.Send(command);
        if (!success)
            return NotFound(new { Message = "Category not found or failed to update." });

        return Ok(new { Message = "Category updated successfully." });
    }

    // 5. Delete Category
    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(
     [FromRoute] string categoryId,
     [FromQuery] string storeId,
     [FromQuery] bool softDelete = true)
    {
        var command = new DeleteCategoryCommand(categoryId, storeId, softDelete);
        var success = await _mediator.Send(command);

        if (!success)
            return NotFound(new { Message = "Category not found." });

        return Ok(new { Message = softDelete ? "Category archived successfully." : "Category permanently deleted." });
    }
}

// Request body helper لعملية التعديل
public record UpdateCategoryDto(
    string StoreId,
    string Name,
    string? Description = null,
    string? ParentId = null,
    string? ImageUrl = null,
    int SortOrder = 0
);