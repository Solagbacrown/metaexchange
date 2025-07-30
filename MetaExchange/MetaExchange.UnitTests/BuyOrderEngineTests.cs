using MetaExchange.Console.Engine;
using MetaExchange.Console.FileReader;
using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using NSubstitute;

namespace MetaExchange.UnitTests;

public class BuyOrderEngineTests
{
    [Theory]
    [InlineData(2, 2, 6000)] // 2 BTC @ 3000
    [InlineData(4, 4, 12500)] // 3 BTC @ 3000 + 1 @ 3500
    [InlineData(10, 8, 26500)] // 3 BTC @ 3000 + 5 @ 3500
    [InlineData(0, 0, 0)] // edge case: zero BTC
    public async Task ExecuteAsync_BuyBTC_VariousCases(decimal btcToBuy, decimal expectedBtc, decimal expectedEur)
    {
        // Arrange
        var fakeOrderBooks = new List<OrderBook>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EurBalance = 20000m,
                BtcBalance = 10,
                Asks =
                [
                    new Ask { Order = new Order { Price = 3000, Amount = 3 } },
                    new Ask { Order = new Order { Price = 3500, Amount = 5 } }
                ],
                Bids = []
            }
        };

        var reader = Substitute.For<IOrderBookReader>();
        reader.ReadOrderBookFromFileAsync(Arg.Any<string>()).Returns(fakeOrderBooks);

        var engine = new BuyOrderEngine(reader);
        var request = new OrderRequest { Amount = btcToBuy, OrderType = OrderType.Buy };

        // Act
        var response = await engine.ExecuteAsync(request);

        // Assert
        Assert.Equal(expectedBtc, response.Summary.TotalBtc);
        Assert.Equal(expectedEur, response.Summary.TotalEur);
    }
}