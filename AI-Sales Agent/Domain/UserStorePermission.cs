namespace AI_Sales_Agent.Domain
{
    public class UserStorePermission
    {
        public string Role { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid StoreId { get; set; }
        public Store Store { get; set; } = null!;
    }
}
