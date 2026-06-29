namespace DockiUp.Application.Dtos
{
    public class ActivityEntryDto
    {
        public required Guid Id { get; set; }
        public required string Action { get; set; }
        public required string Target { get; set; }
        public Guid? ProjectId { get; set; }
        public string? Details { get; set; }
        public string? ActorName { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
