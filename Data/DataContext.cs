using Microsoft.EntityFrameworkCore;
using MyPortfolioBackend.Models;

namespace MyPortfolioBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectImage> ProjectImages { get; set; }
    }
}