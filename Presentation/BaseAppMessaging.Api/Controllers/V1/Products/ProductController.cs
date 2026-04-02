namespace BaseAppMessaging.Api.Controllers.Features.V1.Products;

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using BaseAppMessaging.Application.Common.Exceptions;
using BaseAppMessaging.Application.Common.Models;
using BaseAppMessaging.Application.Features.V1.Products.Commands.CreateProduct;
using BaseAppMessaging.Application.Features.V1.Products.Commands.UpdateProduct;
using BaseAppMessaging.Application.Features.V1.Products.Commands.DeleteProduct;
using BaseAppMessaging.Application.Features.V1.Products.Models;
using BaseAppMessaging.Application.Features.V1.Products.Queries.GetProductById;
using BaseAppMessaging.Application.Features.V1.Products.Queries.GetProducts;
using BaseAppMessaging.Domain.Primitives;


[ApiVersion(1)]
public class ProductController : BaseApiController
{
    public ProductController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Get the paginated product list
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("paginated")]
    [ProducesResponseType(typeof(Result<PaginationResponse<ProductResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductPaginated([FromBody] GetProductsQuery request)
    {
        var result = await Sender.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Get product by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var result = await Sender.Send(new GetProductByIdQuery(id));

        if (result.IsFailure)
            throw new BadRequestException(new List<Error> { result.Error });

        return Ok(result);
    }

    /// <summary>
    /// Create product
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            throw new BadRequestException(new List<Error> { result.Error });

        return CreatedAtAction(nameof(GetProductById), new { Id = result.Value }, result.Value);
    }

    /// <summary>
    /// Update product
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(typeof(Result<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            throw new BadRequestException(new List<Error> { result.Error });

        return CreatedAtAction(nameof(GetProductById), new { Id = result.Value }, result.Value);
    }

    /// <summary>
    /// Delete product
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="BadRequestException"></exception>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await Sender.Send(new DeleteProductCommand(id));

        if (result.IsFailure)
            throw new BadRequestException(new List<Error> { result.Error });

        return Ok(result);
    }

    /// <summary>
    /// Create a random product for testing outbox pattern
    /// </summary>
    [HttpPost("random")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateRandomProduct()
    {
        var random = new Random();
        var productNames = new[] { "Laptop", "Phone", "Tablet", "Monitor", "Keyboard", "Mouse", "Headphones", "Camera" };
        var adjectives = new[] { "Pro", "Ultra", "Max", "Mini", "Lite", "Plus", "Elite", "Basic" };

        var name = $"{productNames[random.Next(productNames.Length)]} {adjectives[random.Next(adjectives.Length)]} {random.Next(1000, 9999)}";
        var description = $"Random product created at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
        var price = Math.Round((decimal)(random.NextDouble() * 1000 + 10), 2);

        var command = new CreateProductCommand(name, description, price);
        var result = await Sender.Send(command);

        if (result.IsFailure)
            throw new BadRequestException(new List<Error> { result.Error });

        return CreatedAtAction(nameof(GetProductById), new { Id = result.Value }, result.Value);
    }
}
