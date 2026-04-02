namespace BaseAppMessaging.Domain.AggregatesModels.Products;

using BaseAppMessaging.Domain.Primitives;


public static class ProductErrors
{
    public static Error NotFound = new(
        "Product.NotFound",
        "Product not found!");
}
