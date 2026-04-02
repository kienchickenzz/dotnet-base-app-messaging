namespace BaseAppMessaging.Application.Features.V1.Products.Commands.DeleteProduct;

using BaseAppMessaging.Application.Common.Messaging;


public sealed record DeleteProductCommand(Guid Id) : ICommand<Guid>;
