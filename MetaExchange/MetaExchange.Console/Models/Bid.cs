namespace MetaExchange.Console.Models;

public class Bid : IExchange
{
    public required Order Order { get; set; }
}