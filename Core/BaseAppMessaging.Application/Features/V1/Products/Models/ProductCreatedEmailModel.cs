namespace BaseAppMessaging.Application.Features.V1.Products.Models;



public class ProductCreatedEmailModel
{
    public string ProductName { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}
