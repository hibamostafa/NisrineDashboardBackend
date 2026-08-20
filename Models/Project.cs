using System.Collections.Generic;

namespace MyPortfolioBackend.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; 
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        // Ensure there are NO [MaxLength(255)] or [StringLength(255)] attributes here:
        public string MainImage { get; set; } = string.Empty;
        public List<ProjectImage> Gallery { get; set; } = new List<ProjectImage>();
    }

    public class ProjectImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int ProjectId { get; set; }
    }
}