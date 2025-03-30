namespace api.Models;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class ApiUser : IdentityUser, IAuditable
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public DateTimeOffset? DateCreated { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; }
}