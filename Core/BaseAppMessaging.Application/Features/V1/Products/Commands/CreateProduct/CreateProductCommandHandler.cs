namespace BaseAppMessaging.Application.Features.V1.Products.Commands.CreateProduct;

using BaseAppMessaging.Application.Common.Messaging;
using BaseAppMessaging.Application.Common.ApplicationServices.Repositories;
using BaseAppMessaging.Domain.AggregatesModels.Products;
using BaseAppMessaging.Domain.Primitives;


public sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    public CreateProductCommandHandler(
        IProductRepository productRepository
    )
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price);

        var result = await _productRepository.AddAsync(product);

        return result.Id;
    }
}
