using MetaExchange.Console.Engine;
using MetaExchange.Console.FileReader;
using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using NSubstitute;

namespace MetaExchange.UnitTests;

public class SellOrderEngineTests
{
    [Theory]
    [InlineData(2, 2, 8000)] // 2 BTC @ 4000
    [InlineData(4, 4, 15000)] // 2 @ 4000 + 2 @ 3500
    [InlineData(0, 0, 0)] // edge case: zero BTC
    public async Task ExecuteAsync_SellBTC_VariousCases(decimal btcToSell, decimal expectedBtc, decimal expectedEur)
    {
        // Arrange
        var fakeOrderBooks = new List<OrderBook>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EurBalance = 20000m,
                BtcBalance = 0,
                Asks = [],
                Bids =
                [
                    new Bid { Order = new Order { Price = 4000, Amount = 2 } },
                    new Bid { Order = new Order { Price = 3500, Amount = 3 } }
                ]
            }
        };

        var reader = Substitute.For<IOrderBookReader>();
        reader.ReadOrderBookFromFileAsync(Arg.Any<string>()).Returns(fakeOrderBooks);

        var engine = new SellOrderEngine(reader);
        var request = new OrderRequest { Amount = btcToSell, OrderType = OrderType.Sell };

        // Act
        var response = await engine.ExecuteAsync(request);

        // Assert
        Assert.Equal(expectedBtc, response.Summary.TotalBtc);
        Assert.Equal(expectedEur, response.Summary.TotalEur);
    }
}