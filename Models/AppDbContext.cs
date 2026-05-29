using Microsoft.EntityFrameworkCore;

namespace User_Panel.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserNote> UserNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserNote>()
                .HasOne(n => n.AppUser)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
