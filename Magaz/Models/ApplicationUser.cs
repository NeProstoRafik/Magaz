using Microsoft.AspNetCore.Identity;

namespace Magaz.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string FullName { get; set; }
    }
}
