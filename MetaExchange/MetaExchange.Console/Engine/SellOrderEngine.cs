using MetaExchange.Console.FileReader;
using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using MetaExchange.Console.Responses;

namespace MetaExchange.Console.Engine;

public class SellOrderEngine(IOrderBookReader orderBookReader) : OrderEngineBase(orderBookReader),  IOrderEngine
{
    public override OrderType SupportedOrderType => OrderType.Sell;
    
    public override async Task<OrderResponse> ExecuteAsync(OrderRequest order)
    {
        var orderBooks = await ReadOrderBookFromFileAsync();
        
        var orderExecutions = new List<OrderExecution>();
        
        var btcRemaining = order.Amount;

        var bids = orderBooks
            .SelectMany(ex => ex.Bids.Select(a => new
            {
                ex.Id,
                ex.EurBalance,
                a.Order.Price,
                a.Order.Amount
            }))
            .OrderByDescending(o => o.Price)
            .ThenByDescending( o => o.Amount)
            .ToList();

        foreach (var bid in bids)
        {
            if (btcRemaining <= 0)
            {
                break;
            }
            
            var maxSellableBtc = Math.Min((decimal)bid.Amount, bid.EurBalance / (decimal)bid.Price);
            var btcToSell = Math.Min(btcRemaining, maxSellableBtc);

            if (btcToSell <= 0)
            {
                continue;
            }

            orderExecutions.Add(new OrderExecution()
            {
                ExchangeId = bid.Id,
                Price = (decimal)bid.Price,
                Amount = btcToSell
            });

            btcRemaining -= btcToSell;
        }

        if (btcRemaining > 0)
        {
            throw new InvalidOperationException($"Unable to sell full BTC amount. Remaining: {btcRemaining} BTC.");
        }

        return CreateOrderResponse(orderExecutions);
    }
}