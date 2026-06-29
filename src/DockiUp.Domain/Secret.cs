namespace DockiUp.Domain
{
    /// <summary>A named secret stored encrypted at rest (AES-256-GCM). Only the ciphertext, nonce and
    /// auth tag are persisted; the plaintext is recoverable only with the configured Vault master key.</summary>
    public class Secret : BaseEntity
    {
        public required string Name { get; init; }
        public required byte[] Ciphertext { get; set; }
        public required byte[] Nonce { get; set; }
        public required byte[] Tag { get; set; }
    }
}
