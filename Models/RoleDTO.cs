namespace api.Models;

using System.ComponentModel.DataAnnotations;

public class RoleDTO
{
    [Required]
    public string Name { get; set; }
}
