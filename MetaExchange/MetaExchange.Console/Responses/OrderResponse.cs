using MetaExchange.Console.Engine;

namespace MetaExchange.Console.Responses;

public class OrderResponse
{
    public required List<OrderExecution> OrderExecutions { get; set; }
    public required OrderExecutionSummary Summary { get; set; }
}