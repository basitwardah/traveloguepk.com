using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using magazine_app.Models;

namespace magazine_app.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Guide> Guides { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Guide configuration
            modelBuilder.Entity<Guide>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.HasIndex(g => g.Slug).IsUnique();
                entity.Property(g => g.Title).IsRequired().HasMaxLength(200);
                entity.Property(g => g.Slug).IsRequired().HasMaxLength(250);
                entity.Property(g => g.CoverImagePath).IsRequired().HasMaxLength(500);
                entity.Property(g => g.PdfPath).IsRequired().HasMaxLength(500);
                entity.Property(g => g.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(g => g.CreatedBy)
                    .WithMany(u => u.CreatedGuides)
                    .HasForeignKey(g => g.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // UserActivity configuration
            modelBuilder.Entity<UserActivity>(entity =>
            {
                entity.HasKey(ua => ua.Id);
                entity.Property(ua => ua.Action).IsRequired().HasMaxLength(100);
                entity.Property(ua => ua.Timestamp).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(ua => ua.User)
                    .WithMany(u => u.Activities)
                    .HasForeignKey(ua => ua.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ua => ua.Guide)
                    .WithMany()
                    .HasForeignKey(ua => ua.GuideId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // LogEntry configuration
            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Level).IsRequired().HasMaxLength(50);
                entity.Property(l => l.Message).IsRequired();
                entity.Property(l => l.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}

