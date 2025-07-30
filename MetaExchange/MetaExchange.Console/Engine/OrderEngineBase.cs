using MetaExchange.Console.FileReader;
using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using MetaExchange.Console.Resources.Path;
using MetaExchange.Console.Responses;

namespace MetaExchange.Console.Engine;

public abstract class OrderEngineBase(IOrderBookReader orderBookReader) : IOrderEngine
{
    protected async Task<List<OrderBook>> ReadOrderBookFromFileAsync()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, Constants.Resources, Constants.Data,  Constants.OrderBook);

        var orderBooks = await orderBookReader.ReadOrderBookFromFileAsync(filePath);
        return orderBooks;  
    }

    protected static OrderResponse CreateOrderResponse(List<OrderExecution> orderExecutions)
    {
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
    
    public abstract OrderType SupportedOrderType { get; }

    public abstract Task<OrderResponse> ExecuteAsync(OrderRequest request);
}