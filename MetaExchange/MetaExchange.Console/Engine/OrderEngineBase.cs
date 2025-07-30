using MetaExchange.Console.FileReader;
using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using MetaExchange.Console.Resources.Path;
using MetaExchange.Console.Responses;

namespace MetaExchange.Console.Engine;

/// <summary>
/// Base class for order engines, providing common functionality for reading order books
/// and creating order response summaries.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OrderEngineBase"/> class.
/// </remarks>
/// <param name="orderBookReader">The order book reader used to read order book data from files.</param>
public abstract class OrderEngineBase(IOrderBookReader orderBookReader) : IOrderEngine
{
    private readonly IOrderBookReader _orderBookReader = orderBookReader;

    /// <summary>
    /// Asynchronously reads the order book data from the file system.
    /// The file path is constructed using the application's base directory and predefined constants.
    /// </summary>
    /// <returns>A list of <see cref="OrderBook"/> objects.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the order book file is not found at the expected location.</exception>
    protected async Task<List<OrderBook>> ReadOrderBookFromFileAsync()
    {
        // Construct the file path using constants for the directory structure
        var filePath = Path.Combine(AppContext.BaseDirectory, Constants.Resources, Constants.Data, Constants.OrderBook);

        // Read the order books from the file asynchronously
        var orderBooks = await _orderBookReader.ReadOrderBookFromFileAsync(filePath);
        return orderBooks;
    }

    /// <summary>
    /// Creates an <see cref="OrderResponse"/> from the provided list of order executions.
    /// This method calculates the total BTC and EUR amounts from the order executions.
    /// </summary>
    /// <param name="orderExecutions">A list of order executions containing details of the orders processed.</param>
    /// <returns>A populated <see cref="OrderResponse"/> containing the order executions and summary.</returns>
    protected static OrderResponse CreateOrderResponse(List<OrderExecution> orderExecutions)
    {
        // Create and return the order response with the order executions and summary
        return new OrderResponse
        {
            OrderExecutions = orderExecutions,
            Summary = new OrderExecutionSummary()
            {
                TotalBtc = orderExecutions.Sum(x => x.Amount),
                TotalEur = orderExecutions.Sum(x => x.Total)
            }
        };
    }

    /// <summary>
    /// Gets the supported order type for this engine (e.g., Buy or Sell).
    /// This property must be implemented in derived classes.
    /// </summary>
    public abstract OrderType SupportedOrderType { get; }

    /// <summary>
    /// Executes an order based on the provided <see cref="OrderRequest"/> and returns the result.
    /// This method must be implemented in derived classes to define the logic for handling specific orders.
    /// </summary>
    /// <param name="request">The order request containing the order details such as amount and order type.</param>
    /// <returns>An <see cref="OrderResponse"/> containing the order execution details.</returns>
    public abstract Task<OrderResponse> ExecuteAsync(OrderRequest request);
}
