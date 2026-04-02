namespace BaseAppMessaging.Application.Features.V1.Products.Commands.UpdateProduct;

using BaseAppMessaging.Application.Common.Messaging;


public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price) : ICommand<Guid>;
