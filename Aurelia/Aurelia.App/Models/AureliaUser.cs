using Microsoft.AspNetCore.Identity;

namespace Aurelia.App.Models
{

    public class AureliaUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }

    }
}
