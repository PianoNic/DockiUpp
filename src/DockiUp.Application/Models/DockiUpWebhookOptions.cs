namespace DockiUp.Application.Models
{
    /// <summary>Webhook options (Komodo-style: secret for git webhook verification).</summary>
    public class DockiUpWebhookOptions
    {
        /// <summary>Secret that must match the one configured in the git provider webhook. If empty, webhook endpoint is disabled or accepts any.</summary>
        public string? WebhookSecret { get; set; }
    }
}
