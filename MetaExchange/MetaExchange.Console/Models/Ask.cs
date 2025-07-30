namespace MetaExchange.Console.Models;

public class Ask : IExchange 
{
    public required Order Order { get; set; }
}