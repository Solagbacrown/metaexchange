using MetaExchange.Console.Engine;
using MetaExchange.Console.Models;
using MetaExchange.Console.Requests;
using MetaExchange.Console.Responses;
using NSubstitute;

namespace MetaExchange.UnitTests;

public class OrderProcessorTests
{
    [Theory]
    [InlineData(3, 9000)]
    [InlineData(5, 15000)]
    [InlineData(0, 0)]
    public async Task ProcessAsync_BuyOrder_ReturnsExpectedSummary(decimal amount, decimal expectedEur)
    {
        // Arrange
        var buyEngine = CreateEngine(OrderType.Buy, amount, expectedEur);
        var processor = new OrderProcessor([buyEngine]);

        var request = new OrderRequest { OrderType = OrderType.Buy, Amount = amount };

        // Act
        var result = await processor.ProcessAsync(request);

        // Assert
        AssertOrderSummary(result, amount, expectedEur);
        await buyEngine.Received(1).ExecuteAsync(Arg.Any<OrderRequest>());
    }

    [Theory]
    [InlineData(2, 8000)]
    [InlineData(4, 16000)]
    public async Task ProcessAsync_SellOrder_ReturnsExpectedSummary(decimal amount, decimal expectedEur)
    {
        // Arrange
        var sellEngine = CreateEngine(OrderType.Sell, amount, expectedEur);
        var processor = new OrderProcessor([sellEngine]);

        var request = new OrderRequest { OrderType = OrderType.Sell, Amount = amount };

        // Act
        var result = await processor.ProcessAsync(request);

        // Assert
        AssertOrderSummary(result, amount, expectedEur);
        await sellEngine.Received(1).ExecuteAsync(Arg.Any<OrderRequest>());
    }

    [Fact]
    public async Task ProcessAsync_NoEngineRegistered_Throws()
    {
        // Arrange: only BUY is registered, request is SELL
        var buyEngine = CreateEngine(OrderType.Buy, 1, 1000);
        var processor = new OrderProcessor([buyEngine]);

        var request = new OrderRequest { OrderType = OrderType.Sell, Amount = 1 };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => processor.ProcessAsync(request));
        Assert.Contains("No engine registered", ex.Message);
    }

    private static IOrderEngine CreateEngine(OrderType type, decimal btc, decimal eur)
    {
        var engine = Substitute.For<IOrderEngine>();
        engine.SupportedOrderType.Returns(type);
        engine.ExecuteAsync(Arg.Any<OrderRequest>()).Returns(new OrderResponse
        {
            Summary = new OrderExecutionSummary { TotalBtc = btc, TotalEur = eur },
            OrderExecutions = []
        });

        return engine;
    }

    private static void AssertOrderSummary(OrderResponse response, decimal expectedBtc, decimal expectedEur)
    {
        Assert.NotNull(response);
        Assert.Equal(expectedBtc, response.Summary.TotalBtc);
        Assert.Equal(expectedEur, response.Summary.TotalEur);
    }
}