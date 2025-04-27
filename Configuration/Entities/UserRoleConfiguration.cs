namespace api.Configuration.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(
            new IdentityUserRole<string>
            {
                RoleId = "b1d6a8f2-7b3b-4a4a-93d5-6f8a78d8e9b2", // Administrator role ID
                UserId = "d2c3a599-5dbb-4fc9-902f-61f27f28b9d0" // Admin user ID
            }
        );
    }
}