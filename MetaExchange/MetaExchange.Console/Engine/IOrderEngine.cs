using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using MetaExchange.Console.Responses;

namespace MetaExchange.Console.Engine;

public interface IOrderEngine
{
    Task<OrderResponse> ExecuteAsync(OrderRequest order);
    OrderType SupportedOrderType { get; }
}