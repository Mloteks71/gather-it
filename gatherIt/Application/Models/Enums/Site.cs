using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Site
{
    JustJoinIt,
    TheProtocolIt,
    SolidJobs,
    PracujPl
}
