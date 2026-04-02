/**
 * Handler for GetProductByIdQuery - retrieves a single product by Id.
 *
 * <p>Uses LINQ with projection extension instead of Specification pattern.</p>
 */
namespace BaseAppMessaging.Application.Features.V1.Products.Queries.GetProductById;

using Microsoft.EntityFrameworkCore;

using BaseAppMessaging.Application.Common.ApplicationServices.Repositories;
using BaseAppMessaging.Application.Common.Messaging;
using BaseAppMessaging.Application.Features.V1.Products.Extensions;
using BaseAppMessaging.Application.Features.V1.Products.Models;
using BaseAppMessaging.Domain.AggregatesModels.Products;
using BaseAppMessaging.Domain.Primitives;


public sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<Result<ProductResponse>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.Query
            .Where(p => p.Id == request.Id)
            .SelectAsResponse()
            .FirstOrDefaultAsync(cancellationToken);

        return product is not null
            ? Result.Success(product)
            : Result.Failure<ProductResponse>(ProductErrors.NotFound);
    }
}
