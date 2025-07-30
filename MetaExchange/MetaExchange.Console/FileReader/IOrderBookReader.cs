namespace MetaExchange.Console.FileReader;

public interface IOrderBookReader
{
    Task<List<OrderBook>> ReadOrderBookFromFileAsync(string path);
}