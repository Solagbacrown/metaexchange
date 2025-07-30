using System.Text.Json.Serialization;

namespace MetaExchange.Console.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderType
{
    Buy,
    Sell
}