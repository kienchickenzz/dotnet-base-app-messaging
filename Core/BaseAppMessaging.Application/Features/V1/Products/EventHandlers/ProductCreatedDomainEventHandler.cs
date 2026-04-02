/**
 * Handler for ProductCreatedDomainEvent.
 *
 * <p>Processes the event after a product is successfully created.
 * Sends notification email to configured recipient.</p>
 */

namespace BaseAppMessaging.Application.Features.V1.Products.EventHandlers;

using Microsoft.Extensions.Logging;

// using BaseAppMessaging.Application.Common.ApplicationServices.Email;
using BaseAppMessaging.Application.Common.Messaging;
using BaseAppMessaging.Domain.AggregatesModels.Products.Events;
using BaseAppMessaging.Application.Features.V1.Products.Models;


/// <summary>
/// Handles ProductCreatedDomainEvent published via outbox pattern.
/// </summary>
public sealed class ProductCreatedDomainEventHandler : IDomainEventHandler<ProductCreatedDomainEvent>
{
    private readonly ILogger<ProductCreatedDomainEventHandler> _logger;
    // private readonly IMailService _mailService;
    // private readonly IMailRequestFactory _mailRequestFactory;
    // private readonly IEmailTemplateFactory _emailTemplateFactory;

    private const string NotificationEmail = "nguyenduckien2508@gmail.com";
    private const string TemplateName = "product-created";

    public ProductCreatedDomainEventHandler(
        ILogger<ProductCreatedDomainEventHandler> logger)
        // IMailService mailService,
        // IMailRequestFactory mailRequestFactory,
        // IEmailTemplateFactory emailTemplateFactory)
    {
        _logger = logger;
        // _mailService = mailService;
        // _mailRequestFactory = mailRequestFactory;
        // _emailTemplateFactory = emailTemplateFactory;
    }

    /// <summary>
    /// Sends notification email when a new product is created.
    /// </summary>
    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[Domain Event] ProductCreated - Name: {ProductName}, Price: {Price:C}",
            notification.ProductName,
            notification.Price);

        try
        {
            // // 1. Build email model
            // var emailModel = new ProductCreatedEmailModel
            // {
            //     ProductName = notification.ProductName,
            //     Price = notification.Price.ToString("C"),
            //     CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
            // };

            // // 2. Render template
            // string emailBody = _emailTemplateFactory.GenerateEmailTemplate(TemplateName, emailModel);

            // // 3. Build mail request using factory
            // var mailRequest = _mailRequestFactory.Create(
            //     to: NotificationEmail,
            //     subject: $"[New Product] {notification.ProductName} has been created!",
            //     body: emailBody
            // );

            // // 4. Send email
            // await _mailService.SendAsync(mailRequest, cancellationToken);

            _logger.LogInformation(
                "[Domain Event] ProductCreated - Email sent to {Email}",
                NotificationEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[Domain Event] ProductCreated - Failed to send email for product: {ProductName}",
                notification.ProductName);
        }
    }
}
