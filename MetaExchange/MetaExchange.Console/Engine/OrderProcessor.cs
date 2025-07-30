using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using MetaExchange.Console.Responses;

namespace MetaExchange.Console.Engine;

/// <summary>
/// Processes orders by delegating to the appropriate order engine based on the order type.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OrderProcessor"/> class.
/// </remarks>
/// <param name="orderEngines">A collection of order engines that can process various order types (e.g., Buy, Sell).</param>
public class OrderProcessor(IEnumerable<IOrderEngine> orderEngines) : IOrderProcessor
{
    private readonly Dictionary<OrderType, IOrderEngine> _orderEngines = orderEngines.ToDictionary(s => s.SupportedOrderType);

    /// <summary>
    /// Processes an order by delegating it to the corresponding order engine based on the order type.
    /// </summary>
    /// <param name="order">The order request, including the type of order and the amount.</param>
    /// <returns>A task representing the asynchronous operation, with the result being an <see cref="OrderResponse"/> containing the order execution details.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no engine is registered for the specified order type.</exception>
    /// <remarks>
    /// The method looks up the appropriate order engine based on the <see cref="OrderType"/> specified in the <see cref="OrderRequest"/>.
    /// If the engine is found, the order is processed and the result is returned.
    /// </remarks>
    public async Task<OrderResponse> ProcessAsync(OrderRequest order)
    {
        // Try to get the corresponding engine for the provided order type
        if (_orderEngines.TryGetValue(order.OrderType, out var orderEngine))
        {
            // Execute the order using the selected engine
            return await orderEngine.ExecuteAsync(order);
        }

        // If no engine is found, throw an exception
        throw new InvalidOperationException($"No engine registered for order type: {order.OrderType}");
    }
}
