namespace MetaExchange.Console.Engine;

public class OrderExecution
{
    public Guid ExchangeId { get; init; }
    public decimal Amount { get; init; }
    public decimal Price { get; init; }
    public decimal Total => Price * Amount;
}