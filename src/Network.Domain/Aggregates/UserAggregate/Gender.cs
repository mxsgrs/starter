using System.Text.Json.Serialization;

namespace Network.Domain.Aggregates.UserAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender
{
    Male,
    Female
}
