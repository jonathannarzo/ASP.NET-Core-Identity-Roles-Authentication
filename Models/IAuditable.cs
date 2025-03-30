namespace api.Models;

public interface IAuditable
{
    DateTimeOffset? DateCreated { get; set; }
    DateTimeOffset? DateUpdated { get; set; }
}