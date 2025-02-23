using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QR_Domain.Entities;

public class QRContext : IdentityDbContext<User, Role, int>
{
    public QRContext(DbContextOptions<QRContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId).IsRequired();
            entity.HasOne(u => u.RefreshToken).WithOne(r => r.User)
                  .HasForeignKey<User>(u => u.RefreshTokenId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            entity.Ignore(e => e.Email);
            entity.Ignore(e => e.NormalizedEmail);
            entity.Ignore(e => e.EmailConfirmed);
            entity.Ignore(e => e.PhoneNumberConfirmed);
            entity.Ignore(e => e.TwoFactorEnabled);
            entity.Ignore(e => e.LockoutEnd);
            entity.Ignore(e => e.LockoutEnabled);
            entity.Ignore(e => e.AccessFailedCount);
        });

        modelBuilder.Entity<Role>(entity =>
            entity.ToTable("roles")
        );

        modelBuilder.Ignore<IdentityRoleClaim<int>>();
        modelBuilder.Ignore<IdentityUserClaim<int>>();
        modelBuilder.Ignore<IdentityUserLogin<int>>();
        modelBuilder.Ignore<IdentityUserToken<int>>();
        modelBuilder.Ignore<IdentityUserRole<int>>();
    }
}