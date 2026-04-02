/**
 * Handler for GetProductsQuery - retrieves paginated list of products.
 *
 * <p>Uses LINQ extensions for filtering, sorting, and pagination
 * instead of Specification pattern.</p>
 */
namespace BaseAppMessaging.Application.Features.V1.Products.Queries.GetProducts;

using BaseAppMessaging.Application.Common.ApplicationServices.Repositories;
using BaseAppMessaging.Application.Common.Extensions;
using BaseAppMessaging.Application.Common.Messaging;
using BaseAppMessaging.Application.Common.Models;
using BaseAppMessaging.Application.Features.V1.Products.Extensions;
using BaseAppMessaging.Application.Features.V1.Products.Models;
using BaseAppMessaging.Domain.Primitives;


public sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PaginationResponse<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<Result<PaginationResponse<ProductResponse>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _productRepository.Query
            .WhereKeywordMatches(request.Keyword)
            .OrderByNewest()
            .SelectAsResponse()
            .ToPaginatedListAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        return Result.Success(result);
    }
}
