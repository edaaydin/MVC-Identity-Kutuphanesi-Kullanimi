using Microsoft.AspNetCore.Identity;

namespace _11_Identity.Models
{
    public class AppRole : IdentityRole
    {
        public DateTime OlusmaTarihi { get; set; }
    }
}
