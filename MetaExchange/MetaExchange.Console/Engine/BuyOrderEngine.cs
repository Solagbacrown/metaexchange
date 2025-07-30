using MetaExchange.Console.FileReader;
using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using MetaExchange.Console.Responses;

namespace MetaExchange.Console.Engine;

public class BuyOrderEngine(IOrderBookReader orderBookReader): OrderEngineBase(orderBookReader), IOrderEngine
{
    public override OrderType SupportedOrderType => OrderType.Buy;

    public override async Task<OrderResponse> ExecuteAsync(OrderRequest order)
    {
        var orderBooks = await ReadOrderBookFromFileAsync();

        var orderExecutions = new List<OrderExecution>();
        
        var asks = orderBooks
            .SelectMany(ex => ex.Asks.Select(a => new
            {
                ex.Id,
                ex.EurBalance,
                ex.BtcBalance,
                a.Order.Price,
                a.Order.Amount
            }))
            .OrderBy(o => o.Price)
            .ThenByDescending( o => o.Amount)
            .ToList();

        var btcRemaining = order.Amount;
        foreach (var ask in asks)
        {
            if (btcRemaining <= 0)
            {
                break;
            }

            var maxBuyableBtc = Math.Min((decimal)ask.Amount, (ask.EurBalance / (decimal)ask.Price));
            var btcToBuy = Math.Min(btcRemaining, maxBuyableBtc);

            if (btcToBuy <= 0)
            {
                continue;
            }

            orderExecutions.Add(new OrderExecution
            {
                ExchangeId = ask.Id,
                Price = (decimal)ask.Price,
                Amount = btcToBuy
            });

            btcRemaining -= btcToBuy;
        }

        return CreateOrderResponse(orderExecutions);
    }
}