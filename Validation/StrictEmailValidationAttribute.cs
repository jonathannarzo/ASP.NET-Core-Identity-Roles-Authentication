using System.ComponentModel.DataAnnotations;

namespace api.Validation
{
    public class StrictEmailValidationAttribute : RegularExpressionAttribute
    {
        public StrictEmailValidationAttribute()
            : base(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
        {
            ErrorMessage = "Invalid email format.";
        }
    }
}