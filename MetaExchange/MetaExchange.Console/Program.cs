using System.Text.Json;
using MetaExchange.Console.CommandHandlers;
using MetaExchange.Console.Engine;
using MetaExchange.Console.FileReader;
using MetaExchange.Console.Models;
using MetaExchange.Console.Output;
using MetaExchange.Console.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace MetaExchange.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.RegisterCommandHandlers();
        services.AddSingleton<IConsoleOutput, ConsoleOutput>();

        await using var provider = services.BuildServiceProvider();

        var output = provider.GetRequiredService<IConsoleOutput>();
        var handler = ResolveHandler(args, provider);
        await handler.ExecuteAsync(args, output);
    }

    private static ICommandHandler ResolveHandler(string[] args, IServiceProvider provider)
    {
        return args.FirstOrDefault() switch
        {
            "-web"     => provider.GetRequiredService<WebCommandHandler>(),
            "-console" => provider.GetRequiredService<ConsoleCommandHandler>(),
            "-help"    => provider.GetRequiredService<HelpCommandHandler>(),
            _          => provider.GetRequiredService<InvalidCommandHandler>()
        };
    }

    public static async Task RunWebApiAsync()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.RegisterDomainServices();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MetaExchange API",
                Version = "v1",
                Description = "Meta-Exchange for Buy/Sell BTC Orders"
            });
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();

        await app.RunAsync();
    }

    public static async Task RunConsoleAppAsync(OrderType orderType, decimal amount)
    {
        var services = new ServiceCollection();
        services.RegisterDomainServices();
        services.AddSingleton<IConsoleOutput, ConsoleOutput>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var scoped = scope.ServiceProvider;

        var output = scoped.GetRequiredService<IConsoleOutput>();
        var processor = scoped.GetRequiredService<IOrderProcessor>();

        var response = await processor.ProcessAsync(new OrderRequest
        {
            OrderType = orderType,
            Amount = amount
        });

        output.WriteLine(JsonSerializer.Serialize(response), ConsoleColor.Magenta);
    }
    
    private static void RegisterCommandHandlers(this IServiceCollection services)
    {
        services.AddSingleton<WebCommandHandler>();
        services.AddSingleton<ConsoleCommandHandler>();
        services.AddSingleton<HelpCommandHandler>();
        services.AddSingleton<InvalidCommandHandler>();
    }

    private static void RegisterDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderProcessor, OrderProcessor>();
        services.AddScoped<IOrderBookReader, OrderBookReader>();
        services.AddScoped<IOrderEngine, BuyOrderEngine>();
        services.AddScoped<IOrderEngine, SellOrderEngine>();
    }
}