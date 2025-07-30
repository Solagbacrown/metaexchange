using MetaExchange.Console.FileReader;
using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using MetaExchange.Console.Responses;

namespace MetaExchange.Console.Engine;

/// <summary>
/// Engine for processing Buy orders from multiple order books.
/// Selects the best prices for buying Bitcoin based on available exchange liquidity.
/// </summary>
public class BuyOrderEngine(IOrderBookReader orderBookReader) : OrderEngineBase(orderBookReader), IOrderEngine
{
    /// <summary>
    /// Specifies the supported order type for this engine (Buy).
    /// </summary>
    public override OrderType SupportedOrderType => OrderType.Buy;

    /// <summary>
    /// Executes a Buy order by selecting the best possible prices from multiple exchanges.
    /// The engine will buy as much BTC as possible from the available asks, constrained by exchange balances.
    /// </summary>
    /// <param name="order">The buy order request, including the amount of BTC desired.</param>
    /// <returns>An <see cref="OrderResponse"/> containing the execution details and totals.</returns>
    /// <exception cref="InvalidOperationException">Thrown if parsing order books fails.</exception>
    /// <remarks>
    /// The method processes the order by selecting the cheapest asks and fulfilling the desired amount of BTC.
    /// The order is processed based on the exchange's EUR and BTC balances.
    /// </remarks>
    public override async Task<OrderResponse> ExecuteAsync(OrderRequest order)
    {
        // Read order books from file asynchronously
        var orderBooks = await ReadOrderBookFromFileAsync();

        // Initialize list to store order executions
        var orderExecutions = new List<OrderExecution>();

        // Flatten and sort the asks from all exchanges
        var asks = orderBooks
            .SelectMany(ex => ex.Asks.Select(a => new
            {
                ex.Id,
                ex.EurBalance,
                ex.BtcBalance,
                a.Order.Price,
                a.Order.Amount
            }))
            .OrderBy(o => o.Price)         // Ascending price (lowest first)
            .ThenByDescending(o => o.Amount)  // Descending amount for better liquidity
            .ToList();

        // Remaining BTC to buy
        var btcRemaining = order.Amount;

        // Loop through asks and buy as much as possible
        foreach (var ask in asks)
        {
            if (btcRemaining <= 0)
            {
                break; // Exit if the full order is fulfilled
            }

            // Determine how much BTC can be bought from this ask based on EUR balance
            var maxBuyableBtc = Math.Min((decimal)ask.Amount, (ask.EurBalance / (decimal)ask.Price));
            var btcToBuy = Math.Min(btcRemaining, maxBuyableBtc); // Take the minimum between remaining and max buyable BTC

            if (btcToBuy <= 0)
            {
                continue; // Skip if we can't afford any more BTC from this ask
            }

            // Record the order execution for this exchange
            orderExecutions.Add(new OrderExecution
            {
                ExchangeId = ask.Id,
                Price = (decimal)ask.Price,
                Amount = btcToBuy
            });

            // Reduce the remaining BTC to buy
            btcRemaining -= btcToBuy;
        }

        // Create and return the order response with all executions
        return CreateOrderResponse(orderExecutions);
    }
}
