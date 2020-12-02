using Microsoft.EntityFrameworkCore;

namespace BlazorChecklist.Shared
{
    public class ChecklistContext : DbContext
    {
        public ChecklistContext(DbContextOptions<ChecklistContext> options) : base(options) { }

        public DbSet<Checklist> Checklists { get; set; }
        public DbSet<Item> Items { get; set; }

    }
}