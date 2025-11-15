using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace Repository.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Guide> Guides { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.Slug).IsUnique();
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Slug).IsRequired().HasMaxLength(50);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Guide configuration
            modelBuilder.Entity<Guide>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.HasIndex(g => g.Slug).IsUnique();
                entity.Property(g => g.Title).IsRequired().HasMaxLength(200);
                entity.Property(g => g.Slug).IsRequired().HasMaxLength(250);
                entity.Property(g => g.CoverImagePath).IsRequired().HasMaxLength(500);
                entity.Property(g => g.PdfPath).IsRequired().HasMaxLength(500);
                entity.Property(g => g.CurrentPrice).HasColumnType("decimal(18,2)");
                entity.Property(g => g.OldPrice).HasColumnType("decimal(18,2)");
                entity.Property(g => g.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(g => g.CreatedBy)
                    .WithMany(u => u.CreatedGuides)
                    .HasForeignKey(g => g.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(g => g.Category)
                    .WithMany(c => c.Guides)
                    .HasForeignKey(g => g.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
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

            // Favorite configuration
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.UserId).IsRequired();
                entity.Property(f => f.GuideId).IsRequired();
                entity.HasIndex(f => new { f.UserId, f.GuideId }).IsUnique();
                
                entity.HasOne(f => f.User)
                    .WithMany()
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(f => f.Guide)
                    .WithMany()
                    .HasForeignKey(f => f.GuideId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

