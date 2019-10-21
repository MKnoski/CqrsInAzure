using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CqrsInAzure.Candidates.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RequestStatus
    {
        Unknown = 0,
        Initialized,
        Succeeded,
        Failed
    }
}