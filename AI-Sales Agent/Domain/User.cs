
using Microsoft.AspNetCore.Identity;

namespace AI_Sales_Agent.Domain
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateTime? LastLogin {  get; set; } 

        //Subscription
        public Subscription? Subscription { get; set; }

        //store
        public ICollection<Store> Stores { get; set; } = new List<Store>();

        //store permissions
        public ICollection<UserStorePermission> StorePermissions { get; set; } = new List<UserStorePermission>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
