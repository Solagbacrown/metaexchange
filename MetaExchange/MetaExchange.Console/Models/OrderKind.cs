using System.Text.Json.Serialization;

namespace MetaExchange.Console.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderKind
{
    Limit,
    Market,
    Stop,
    StopLimit,
    TakeProfit,
    OCO
}