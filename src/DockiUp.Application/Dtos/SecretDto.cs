namespace DockiUp.Application.Dtos
{
    /// <summary>Secret metadata for listing. The plaintext value is never exposed by the API.</summary>
    public class SecretDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
