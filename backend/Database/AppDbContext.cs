using Microsoft.EntityFrameworkCore;

namespace Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Snippet> Snippets { get; set; }
    public DbSet<Submission> Submissions { get; set; }
}
