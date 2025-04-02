using cosmeticClinic.Backend.Controllers.Base;
using cosmeticClinic.Business;
using cosmeticClinic.DTOs;
using cosmeticClinic.DTOs.Common;
using cosmeticClinic.DTOs.Product;
using cosmeticClinic.Settings;
using cosmeticClinic.Settings.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace cosmeticClinic.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Manage Product operations")]
public class ProductsController : BaseController
{
    private readonly ProductService _ProductService;
    private static readonly string[] SearchFields = new[] { "name","category", "price", "stockQuantity", "createdAt" };

    public ProductsController(
        ILogger<ProductsController> logger,
        ProductService ProductService) : base(logger)
    {
        _ProductService = ProductService;
    }

    [HttpGet]
    [RequirePermission(Permission.ViewProducts)]
    [SwaggerOperation(Summary = "Get all Products")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all Products", typeof(IEnumerable<ProductDto>))]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        return await HandleResponse(
            () => _ProductService.GetAllProductsAsync(),
            "Successfully retrieved all Products");
    }
    
    [HttpGet("getByCategory")]
    [RequirePermission(Permission.ViewProducts)]
    [SwaggerOperation(Summary = "Get all Products By Category")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all Products By Category", typeof(IEnumerable<ProductDto>))]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory([SwaggerParameter("Get Products By Category Available Categories  :\n    Skincare,\n    Haircare,\n    Bodycare,\n    SunProtection,\n    AcneTreatment,\n    Brightening,\n    LipCare,\n    EyeCare,\n    PostTreatmentCare") ]string category)
    {
        return await HandleResponse(
            () => _ProductService.GetProductsByCategoryAsync(category),
            "Successfully retrieved all Products");
    }
    
    [HttpGet("paginated")]
    [RequirePermission(Permission.ViewProducts)]
    [SwaggerOperation(Summary = "Get paginated Products",
    Description = "Retrieves a paginated list of Products. Results are ordered according to specified criteria. for example it must be[\"name\", \"category\", \"price\" , \"stockQuantity\" , \"createdAt\" ]  "
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns paginated Products")]
    public async Task<ActionResult<PaginatedResponseDto <ProductDto>>>
        GetPaginated([FromQuery] [SwaggerParameter("Pagination parameters")]
            PaginationDto pagination)
    {
        if (pagination.PageNumber < 1 || pagination.PageSize < 1)
        {
            return BadRequest(new { message = "Page number and page size must be greater than 0" });
        }

        if (!string.IsNullOrEmpty(pagination.OrderBy) && !SearchFields.Contains(pagination.OrderBy))
        {
            return BadRequest(new { message = $"OrderBy must be one of: {string.Join(", ", SearchFields)}" });
        }
        
        return await HandleResponse(
            () => _ProductService.GetAllProductsAsync(pagination.PageNumber, pagination.PageSize, pagination?.OrderBy, pagination.Ascending),
            "Successfully retrieved paginated Products");
    }

    
    [HttpGet("{id}")]
    [RequirePermission(Permission.ViewProducts)]
    [SwaggerOperation(Summary = "Get a Product by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the requested Product", typeof(ProductDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
    public async Task<ActionResult<ProductDto?>> GetById(string id)
    {
        return await HandleResponse(
            () => _ProductService.GetProductByIdAsync(id),
            $"Successfully retrieved Product with ID: {id}");
    }
    
    
    [HttpPost("search")]
    [RequirePermission(Permission.ViewProducts)]
    [SwaggerOperation(Summary = "Search Products  using specified criteria.",
     Description = " Available search field:name")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns matching Products", typeof(IEnumerable<ProductDto>))]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Search([FromBody] [SwaggerRequestBody("Search criteria")]
        SearchCriteriaDto criteria)
    {
        return await HandleResponse(
            () => _ProductService.SearchProductsAsync(criteria),
            "Successfully performed Product search");
    }
    
    
    [HttpPost]
    [RequirePermission(Permission.CreateProduct)]
    [SwaggerOperation(Summary = "Create a new Product")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product created successfully", typeof(ProductDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<ActionResult<ProductDto>> Create(ProductCreateDto ProductCreateDto)
    {
        return await HandleResponse(
            () => _ProductService.AddProductAsync(ProductCreateDto),
            "Product created successfully");
    }

    
    [HttpPut("{id}")]
    [RequirePermission(Permission.MangeProduct)]
    [SwaggerOperation(Summary = "Update a Product")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product updated successfully", typeof(ProductDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
    public async Task<ActionResult<ProductDto?>> Update(string id, ProductDto ProductDto)
    {
        return await HandleResponse(
            () => _ProductService.UpdateProductAsync(id, ProductDto),
            $"Successfully updated Product with ID: {id}");
    }

    
    [HttpDelete("{id}")]
    [RequirePermission(Permission.DeleteProduct)]
    [SwaggerOperation(Summary = "Delete a Product")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
    public async Task<ActionResult<bool>> Delete(string id)
    {
        return await HandleResponse(
            () => _ProductService.SoftDeleteProductAsync(id),
            $"Successfully deleted Product with ID: {id}");
    }

}