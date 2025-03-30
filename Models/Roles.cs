namespace api.Models;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class Roles : IdentityRole
{
    [NotMapped]
    public virtual ICollection<UserRole> UserRoles { get; set; }
}