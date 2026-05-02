using System.Text.Json.Serialization;

namespace Connect.Common
{
    public record Error(string Code, string Description, ErrorType Type)
    {
        public static Error None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static Error NullValue = new("Error.NullValue", "The specified result value is null.", ErrorType.Failure);
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4
    }
}
