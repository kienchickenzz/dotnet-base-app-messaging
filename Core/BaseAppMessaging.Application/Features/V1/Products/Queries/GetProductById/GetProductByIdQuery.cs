namespace BaseAppMessaging.Application.Features.V1.Products.Queries.GetProductById;

using BaseAppMessaging.Application.Common.Messaging;
using BaseAppMessaging.Application.Features.V1.Products.Models;


public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductResponse>;
