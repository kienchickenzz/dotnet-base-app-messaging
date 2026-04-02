namespace BaseAppMessaging.Application.Common.ApplicationServices.Messaging;

using System.Text;
using System.Text.Json;

/// <summary>
/// Generic wrapper for messages sent through the message broker.
/// </summary>
/// <typeparam name="T">The type of the message payload.</typeparam>
public class Message<T>
{
    /// <summary>
    /// Gets or sets the message metadata for tracing and correlation.
    /// </summary>
    public MetaData? MetaData { get; set; }

    /// <summary>
    /// Gets or sets the actual message payload.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Serializes the message to a JSON string.
    /// </summary>
    /// <returns>JSON representation of the message.</returns>
    public string SerializeObject()
    {
        return JsonSerializer.Serialize(this);
    }

    /// <summary>
    /// Serializes the message to UTF-8 encoded bytes.
    /// </summary>
    /// <returns>UTF-8 byte array of the serialized message.</returns>
    public byte[] GetBytes()
    {
        return Encoding.UTF8.GetBytes(SerializeObject());
    }
}
