namespace BaseAppMessaging.Application.Features.V1.Products.Commands.CreateProduct;

using BaseAppMessaging.Application.Common.Messaging;


public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price)
    : ICommand<Guid>;
