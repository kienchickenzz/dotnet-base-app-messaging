namespace BaseAppMessaging.Application.Features.V1.Products.Queries.GetProducts;

using BaseAppMessaging.Application.Common.Messaging;
using BaseAppMessaging.Application.Common.Models;
using BaseAppMessaging.Application.Features.V1.Products.Models;


public sealed class GetProductsQuery : PaginationFilter, IQuery<PaginationResponse<ProductResponse>>
{
}
