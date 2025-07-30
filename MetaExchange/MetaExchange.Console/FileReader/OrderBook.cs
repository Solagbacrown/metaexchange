using MetaExchange.Console.Models;

namespace MetaExchange.Console.FileReader;

public class OrderBook
{
    public Guid Id { get; init; }
    public decimal EurBalance { get; init; }
    public decimal BtcBalance { get; init; }
    public DateTimeOffset AcqTime { get; init; }
    public required List<Bid> Bids { get; set; }
    public required List<Ask> Asks { get; set; }
}