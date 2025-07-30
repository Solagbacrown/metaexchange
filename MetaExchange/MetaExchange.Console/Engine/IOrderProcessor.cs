using MetaExchange.Console.Requests;
using MetaExchange.Console.Responses;

namespace MetaExchange.Console.Engine;

public interface IOrderProcessor
{
    Task<OrderResponse> ProcessAsync(OrderRequest order);
}