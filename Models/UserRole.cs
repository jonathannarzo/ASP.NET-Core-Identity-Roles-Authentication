namespace api.Models;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class UserRole : IdentityUserRole<string>
{
    public virtual ApiUser User { get; set; }
    public virtual Roles Role { get; set; }
}