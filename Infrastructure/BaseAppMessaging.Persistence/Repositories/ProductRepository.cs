/**
 * Repository implementation for Product aggregate.
 *
 * <p>Inherits all CRUD operations from base Repository.
 * Add domain-specific query methods here if needed.</p>
 */
namespace BaseAppMessaging.Persistence.Repositories;

using BaseAppMessaging.Application.Common.ApplicationServices.Repositories;
using BaseAppMessaging.Domain.AggregatesModels.Products;
using BaseAppMessaging.Persistence.Common;
using BaseAppMessaging.Persistence.DatabaseContext;


public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
