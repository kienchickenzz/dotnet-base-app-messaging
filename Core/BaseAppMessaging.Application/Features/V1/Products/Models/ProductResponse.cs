namespace BaseAppMessaging.Application.Features.V1.Products.Models;

using BaseAppMessaging.Application.Common;


public sealed class ProductResponse : IAuditResponse
{
    public Guid Id { get; init; }
    public Guid CreatedBy { get; init; }
    public DateTime CreatedOn { get; init; }
    public Guid? LastModifiedBy { get; init; }
    public DateTime? LastModifiedOn { get; init; }
    public DateTime? DeletedOn { get; init; }
    public Guid? DeletedBy { get; init; }

    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
}
