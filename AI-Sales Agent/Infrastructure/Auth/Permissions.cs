namespace AI_Sales_Agent.Infrastructure.Auth
{
    public static class Permissions
    {
        public const string UsersManage = "Users.Manage";
        public const string StoresRead = "Stores.Read";
        public const string StoresManage = "Stores.Manage";
        public const string SubscriptionsManage = "Subscriptions.Manage";
        public const string OrganizationsManage = "Organizations.Manage";

        public static readonly string[] All =
        {
            UsersManage,
            StoresRead,
            StoresManage,
            SubscriptionsManage,
            OrganizationsManage
        };
    }
}
