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

/// <summary>
/// Entry point of the MetaExchange Console & Web API application.
/// It provides the logic for running both the console application and the Web API.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// Depending on the provided command-line arguments, this method runs either the console application or the Web API.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        // Setup dependency injection container
        var services = new ServiceCollection();
        services.RegisterCommandHandlers();
        services.AddSingleton<IConsoleOutput, ConsoleOutput>();

        // Build the service provider and resolve the handler
        await using var provider = services.BuildServiceProvider();
        var output = provider.GetRequiredService<IConsoleOutput>();
        var handler = ResolveHandler(args, provider);

        // Execute the handler
        await handler.ExecuteAsync(args, output);
    }

    /// <summary>
    /// Resolves and retrieves the appropriate command handler based on the input arguments.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <param name="provider">The dependency injection service provider.</param>
    /// <returns>An instance of <see cref="ICommandHandler"/> based on the provided arguments.</returns>
    private static ICommandHandler ResolveHandler(string[] args, IServiceProvider provider)
    {
        return args.FirstOrDefault() switch
        {
            "-web" => provider.GetRequiredService<WebCommandHandler>(),
            "-console" => provider.GetRequiredService<ConsoleCommandHandler>(),
            "-help" => provider.GetRequiredService<HelpCommandHandler>(),
            _ => provider.GetRequiredService<InvalidCommandHandler>()
        };
    }

    /// <summary>
    /// Runs the Web API to expose endpoints for processing BTC orders.
    /// Sets up and configures Swagger for API documentation.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task RunWebApiAsync()
    {
        var builder = WebApplication.CreateBuilder();

        // Register domain services
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

        // Enable Swagger UI
        app.UseSwagger();
        app.UseSwaggerUI();

        // Map controllers for the Web API
        app.MapControllers();

        // Run the Web API
        await app.RunAsync();
    }

    /// <summary>
    /// Runs the console application to process a buy or sell order.
    /// Uses the provided <see cref="OrderType"/> and <see cref="amount"/> for the order.
    /// </summary>
    /// <param name="orderType">The type of the order (Buy/Sell).</param>
    /// <param name="amount">The amount of BTC to buy or sell.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task RunConsoleAppAsync(OrderType orderType, decimal amount)
    {
        var services = new ServiceCollection();
        services.RegisterDomainServices();
        services.AddSingleton<IConsoleOutput, ConsoleOutput>();

        // Build the service provider and create a scope for resolution
        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var scoped = scope.ServiceProvider;

        // Resolve required services
        var output = scoped.GetRequiredService<IConsoleOutput>();
        var processor = scoped.GetRequiredService<IOrderProcessor>();

        // Process the order request and output the result
        var response = await processor.ProcessAsync(new OrderRequest
        {
            OrderType = orderType,
            Amount = amount
        });

        // Print the response in the console
        output.WriteLine(JsonSerializer.Serialize(response), ConsoleColor.Magenta);
    }

    /// <summary>
    /// Registers the command handlers for the console application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the handlers with.</param>
    private static void RegisterCommandHandlers(this IServiceCollection services)
    {
        services.AddSingleton<WebCommandHandler>();
        services.AddSingleton<ConsoleCommandHandler>();
        services.AddSingleton<HelpCommandHandler>();
        services.AddSingleton<InvalidCommandHandler>();
    }

    /// <summary>
    /// Registers the domain services, including order processing, book reading, and order engine.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the services with.</param>
    private static void RegisterDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderProcessor, OrderProcessor>();
        services.AddScoped<IOrderBookReader, OrderBookReader>();
        services.AddScoped<IOrderEngine, BuyOrderEngine>();
        services.AddScoped<IOrderEngine, SellOrderEngine>();
    }
}
