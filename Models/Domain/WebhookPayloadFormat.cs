namespace PuckDrop.Models.Domain;

/// <summary>
/// Defines the payload format types for webhook delivery.
/// </summary>
public enum WebhookPayloadFormat
{
    /// <summary>
    /// Generic JSON payload format.
    /// </summary>
    Generic = 0,

    /// <summary>
    /// Discord-compatible webhook payload with embeds.
    /// </summary>
    Discord = 1,

    /// <summary>
    /// Home Assistant webhook payload format.
    /// </summary>
    HomeAssistant = 2
}
