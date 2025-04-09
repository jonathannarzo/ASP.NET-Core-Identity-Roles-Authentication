namespace api.Models;

using api.Models;
using api.Configuration.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApiUser>
// Inheritance for including roles in query
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        // Form postgres sql date time error
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new RoleConfiguration());

        builder.Entity<ApiUser>(b =>
        {
            b.HasMany(e => e.UserRoles)
            .WithOne(e => e.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();
        });

        builder.Entity<Roles>(b =>
        {
            b.HasMany(e => e.UserRoles)
            .WithOne(e => e.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        /**
        Method used for inserting and updating DateCreated and DateUpdated fields
        */

        // Get the added entries
        var insertedEntries = this.ChangeTracker.Entries().Where(x => x.State == EntityState.Added).Select(x => x.Entity);

        foreach (var insertedEntry in insertedEntries)
        {
            // Check if the entity implements IAuditable
            if (insertedEntry is IAuditable auditableEntity)
            {
                auditableEntity.DateCreated = DateTimeOffset.UtcNow;
                auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
            }
        }

        // Get the modified entries
        var modifiedEntries = this.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).Select(x => x.Entity);

        foreach (var modifiedEntry in modifiedEntries)
        {
            // Check if the entity implements IAuditable
            if (modifiedEntry is IAuditable auditableEntity)
            {
                auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<api.Models.Roles> Roles { get; set; } = default!;
}