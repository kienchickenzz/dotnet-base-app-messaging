namespace BaseAppMessaging.Application.Common.ApplicationServices.Repositories;

using BaseAppMessaging.Domain.AggregatesModels.Products;


/// <summary>
/// Repository interface for Product aggregate operations.
/// </summary>
/// <remarks>
/// Extends the generic repository with Product-specific query methods.
/// </remarks>
public interface IProductRepository : IRepository<Product>
{
}
