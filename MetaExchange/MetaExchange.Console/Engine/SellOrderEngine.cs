using MetaExchange.Console.FileReader;
using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using MetaExchange.Console.Responses;

namespace MetaExchange.Console.Engine;

/// <summary>
/// Engine for processing Sell orders from multiple order books.
/// This engine selects the highest bids for selling Bitcoin based on available exchange liquidity.
/// </summary>
public class SellOrderEngine(IOrderBookReader orderBookReader) : OrderEngineBase(orderBookReader), IOrderEngine
{
    /// <summary>
    /// Specifies the supported order type for this engine (Sell).
    /// </summary>
    public override OrderType SupportedOrderType => OrderType.Sell;

    /// <summary>
    /// Executes a Sell order by selecting the highest paying bids from multiple exchanges.
    /// The engine will sell as much BTC as possible based on the available bids and exchange balances.
    /// </summary>
    /// <param name="order">The sell order request, including the amount of BTC to sell.</param>
    /// <returns>An <see cref="OrderResponse"/> containing the execution details and totals.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the order cannot be fully fulfilled.</exception>
    /// <remarks>
    /// The method processes the order by selecting the highest bids and fulfilling the desired amount of BTC.
    /// The order is processed based on the exchange's EUR and BTC balances.
    /// </remarks>
    public override async Task<OrderResponse> ExecuteAsync(OrderRequest order)
    {
        // Read order books from file asynchronously
        var orderBooks = await ReadOrderBookFromFileAsync();

        // Initialize list to store order executions
        var orderExecutions = new List<OrderExecution>();

        // Flatten and sort the bids from all exchanges
        var bids = orderBooks
            .SelectMany(ex => ex.Bids.Select(a => new
            {
                ex.Id,
                ex.EurBalance,
                a.Order.Price,
                a.Order.Amount
            }))
            .OrderByDescending(o => o.Price)   // Descending price (highest first)
            .ThenByDescending(o => o.Amount)  // Descending amount for better liquidity
            .ToList();

        // Remaining BTC to sell
        var btcRemaining = order.Amount;

        // Loop through bids and sell as much as possible
        foreach (var bid in bids)
        {
            if (btcRemaining <= 0)
            {
                break; // Exit if the full order is fulfilled
            }

            // Determine how much BTC can be sold from this bid based on EUR balance
            var maxSellableBtc = Math.Min((decimal)bid.Amount, bid.EurBalance / (decimal)bid.Price);
            var btcToSell = Math.Min(btcRemaining, maxSellableBtc); // Take the minimum between remaining and max sellable BTC

            if (btcToSell <= 0)
            {
                continue; // Skip if we can't sell any more BTC to this bid
            }

            // Record the order execution for this exchange
            orderExecutions.Add(new OrderExecution()
            {
                ExchangeId = bid.Id,
                Price = (decimal)bid.Price,
                Amount = btcToSell
            });

            // Reduce the remaining BTC to sell
            btcRemaining -= btcToSell;
        }

        // If any BTC is left to sell, throw an exception
        if (btcRemaining > 0)
        {
            throw new InvalidOperationException($"Unable to sell full BTC amount. Remaining: {btcRemaining} BTC.");
        }

        // Create and return the order response with all executions
        return CreateOrderResponse(orderExecutions);
    }
}