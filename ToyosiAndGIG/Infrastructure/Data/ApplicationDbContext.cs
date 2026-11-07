using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<AccountConfirmation> AccountConfirmations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<AboutInfo> AboutInfos { get; set; }
        public DbSet<Employee> Employees { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.Property(u => u.PhoneNumber).HasMaxLength(11);
                entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.DeletedBy).HasMaxLength(200);
                entity.Property(u => u.LastUpdatedBy).HasMaxLength(200);

                // Every query against Users automatically excludes soft-deleted rows —
                // GetUserByEmailAsync, login, FindByEmailAsync, etc. all respect this
                // without needing to remember to add a WHERE clause everywhere.
                entity.HasQueryFilter(u => !u.IsDeleted);
            });

            builder.Entity<AccountConfirmation>(entity =>
            {
                entity.Property(a => a.OTP).HasMaxLength(10).IsRequired();

                entity.HasOne(a => a.User)
                      .WithMany()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Speeds up the "find latest unused OTP for this user+purpose" lookup
                entity.HasIndex(a => new { a.UserId, a.Purpose, a.IsUsed });
            });

            builder.Entity<RefreshToken>(entity =>
            {
                entity.Property(r => r.Token).HasMaxLength(200).IsRequired();

                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Speeds up lookups by token string and per-user active-token queries
                entity.HasIndex(r => r.Token).IsUnique();
                entity.HasIndex(r => new { r.UserId, r.IsRevoked });
            });

            builder.Entity<Employee>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId).IsUnique(); // one Employee row per User
            });
        }

       
    }
}
