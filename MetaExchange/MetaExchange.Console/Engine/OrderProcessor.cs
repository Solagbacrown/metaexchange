using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using MetaExchange.Console.Responses;

namespace MetaExchange.Console.Engine;

public class OrderProcessor : IOrderProcessor
{
    private readonly Dictionary<OrderType, IOrderEngine> _orderEngines;

    public OrderProcessor(IEnumerable<IOrderEngine> orderEngines)
    {
        _orderEngines = orderEngines.ToDictionary(s => s.SupportedOrderType);
    }

    public async Task<OrderResponse> ProcessAsync(OrderRequest order)
    {
        if (_orderEngines.TryGetValue(order.OrderType, out var orderEngine))
        {
            return await orderEngine.ExecuteAsync(order);
        }

        throw new InvalidOperationException($"No engine registered for order type: {order.OrderType}");
    }
}