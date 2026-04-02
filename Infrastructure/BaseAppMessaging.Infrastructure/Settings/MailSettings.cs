/**
 * Mail configuration settings.
 *
 * <p>Contains provider selection and provider-specific configurations.
 * Designed for extensibility with multiple email providers.</p>
 */

namespace BaseAppMessaging.Infrastructure.Settings;

/// <summary>
/// Email provider options.
/// </summary>
public enum EmailProviderEnum
{
    /// <summary>
    /// Fake provider for development (log only, no actual sending).
    /// </summary>
    Fake,

    /// <summary>
    /// MailKit SMTP provider (production).
    /// </summary>
    MailKit
}

/// <summary>
/// Root mail configuration.
/// </summary>
public class MailSettings
{
    public const string SectionName = "MailSettings";

    /// <summary>
    /// Active email provider.
    /// </summary>
    public EmailProviderEnum Provider { get; set; } = EmailProviderEnum.MailKit;

    /// <summary>
    /// MailKit SMTP configuration.
    /// </summary>
    public MailKitSettings? Mailkit { get; set; }
}

/// <summary>
/// MailKit SMTP provider configuration.
/// </summary>
public class MailKitSettings
{
    public string? From { get; set; }

    public string? Host { get; set; }

    public int Port { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? DisplayName { get; set; }
}
