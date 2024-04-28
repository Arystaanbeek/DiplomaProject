using Microsoft.AspNetCore.Identity;

namespace DiplomaProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        /*public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }*/
        public bool IsSubscribed { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
    }
}
