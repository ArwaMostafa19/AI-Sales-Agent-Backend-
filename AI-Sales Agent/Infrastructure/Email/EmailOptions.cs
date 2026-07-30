namespace AI_Sales_Agent.Infrastructure.Email
{
    public class EmailOptions
    {
        public const string SectionName = "Email";

        public string From { get; set; } = string.Empty;
        public string DisplayName { get; set; } = "AI Sales Agent";
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }
}
