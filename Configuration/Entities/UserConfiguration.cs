namespace api.Configuration.Entities;

using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<ApiUser>
{
    public void Configure(EntityTypeBuilder<ApiUser> builder)
    {
        var user = new ApiUser
        {
            Id = Guid.Parse("d2c3a599-5dbb-4fc9-902f-61f27f28b9d0").ToString(),
            FirstName = "admin",
            LastName = "example",
            PhoneNumber = "0413662048",
            UserName = "admin@example.com",
            NormalizedUserName = "ADMIN@EXAMPLE.COM",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.Parse("4b2b6e1c-3243-45a2-9948-fb6e5bb85e88").ToString(),
            ConcurrencyStamp = Guid.Parse("2472bef8-22c7-4e09-bc30-098e4bbbcaf9").ToString(),
            PasswordHash = "AQAAAAIAAYagAAAAEAwbkjEDXvjQAg1uRhteKl/uVkQnKMEnF9J8Qy0VS44yaOzWmuB0gEoRJhIP7tlWkA==", // P@ssw0rd1
            DateCreated = new DateTimeOffset(new DateTime(2024, 4, 26, 0, 0, 0, DateTimeKind.Utc), TimeSpan.Zero),
            DateUpdated = new DateTimeOffset(new DateTime(2024, 4, 26, 0, 0, 0, DateTimeKind.Utc), TimeSpan.Zero)
        };

        builder.HasData(user);
    }
}