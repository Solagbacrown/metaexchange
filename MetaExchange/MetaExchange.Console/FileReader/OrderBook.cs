using MetaExchange.Console.Models;

namespace MetaExchange.Console.FileReader;

public class OrderBook
{
    public Guid Id { get; set; }
    public decimal EurBalance { get; set; }
    public decimal BtcBalance { get; set; }
    public DateTimeOffset AcqTime { get; set; }
    public required List<Bid> Bids { get; set; }
    public required List<Ask> Asks { get; set; }
}