namespace MetaExchange.Console.Models;

public class Order
{
    public Guid? Id { get; set; }
    public DateTime Time { get; set; }
    public OrderType Type { get; set; }
    public OrderKind Kind { get; set; }

    public double Amount { get; set; }
    public double Price { get; set; }
}