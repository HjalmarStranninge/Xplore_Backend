using CC_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CC_Backend.Data
{
    public class NatureAIContext : IdentityDbContext<ApplicationUser>
    {


        public DbSet<Category> Categories { get; set; }
        public DbSet<Friends> Friends { get; set; }
        public DbSet<Geodata> GeoData { get; set; }
        public DbSet<Stamp> Stamps { get; set; }
        public DbSet<StampCollected> StampsCollected { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }

        public NatureAIContext(DbContextOptions<NatureAIContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure to call base method
            base.OnModelCreating(modelBuilder);

            // Define primary key for IdentityUserLogin<string> entity
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });

            // Define primary key for IdentityUserRole<string> entity
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });

            modelBuilder.Entity<Friends>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.FriendId1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friends>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.FriendId2)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
            .HasOne(c => c.StampCollected)
            .WithMany(s => s.Comments)
            .HasForeignKey(c => c.StampCollectedId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.StampCollected)
                .WithMany(s => s.Likes)
                .HasForeignKey(l => l.StampCollectedId);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId);
        }
    }
}