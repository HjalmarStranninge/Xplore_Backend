using CC_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CC_Backend.Data
{
    public class NatureAIContext : IdentityDbContext<ApplicationUser>
    {


        public DbSet<Category> Categories { get; set; }
        public DbSet<Friends> Friends  { get; set; }
        public DbSet<Geodata> GeoData { get; set; }
        public DbSet<Stamp> Stamps { get; set; }
        public DbSet<StampCollected> StampsCollected { get; set; }

        public NatureAIContext(DbContextOptions<NatureAIContext> options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder); // Ensure to call base method

            // Define primary key for IdentityUserLogin<string> entity
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });

            // Define primary key for IdentityUserRole<string> entity
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });

            modelBuilder.Entity<Friends>()
                .HasNoKey()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.FriendId1)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Friends>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.FriendId2)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
