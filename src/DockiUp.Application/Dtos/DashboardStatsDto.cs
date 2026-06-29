namespace DockiUp.Application.Dtos
{
    /// <summary>At-a-glance counts for the dashboard (Komodo/KRINT-style overview).</summary>
    public class DashboardStatsDto
    {
        /// <summary>Projects managed by DockiUp (rows in the database).</summary>
        public required int TotalProjects { get; set; }
        public required int TotalContainers { get; set; }
        public required int RunningContainers { get; set; }
        public required IReadOnlyList<ActivityEntryDto> RecentActivity { get; set; }
    }
}
