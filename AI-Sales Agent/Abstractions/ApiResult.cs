namespace AI_Sales_Agent.Abstractions
{
    public record ApiResult(bool Succeeded, string Message, IEnumerable<string>? Errors = null)
    {
        public static ApiResult Success(string message) => new(true, message);

        public static ApiResult Failure(string message, IEnumerable<string>? errors = null) =>
            new(false, message, errors);
    }
}
