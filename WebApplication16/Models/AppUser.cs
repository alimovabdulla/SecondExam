using Microsoft.AspNetCore.Identity;

namespace WebApplication16.Models
{
    public class AppUser:IdentityUser
    {
        public string OTP { get; set; }
    }
}
