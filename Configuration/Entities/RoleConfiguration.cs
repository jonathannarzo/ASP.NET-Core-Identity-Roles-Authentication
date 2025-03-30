namespace api.Configuration.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = Guid.Parse("9e4f49fe-0786-44c6-9061-53d2aa84fab3").ToString(),
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole
            {
                Id = Guid.Parse("b1d6a8f2-7b3b-4a4a-93d5-6f8a78d8e9b2").ToString(),
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            }
        );
    }
}
