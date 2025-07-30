using MetaExchange.Console.Models;
using MetaExchange.Console.Output;

namespace MetaExchange.Console.CommandHandlers;

public class ConsoleCommandHandler : ICommandHandler
{
    public async Task ExecuteAsync(string[] args, IConsoleOutput consoleOutput)
    {
        if (args.Length == 3 &&
            Enum.TryParse<OrderType>(args[1], ignoreCase: true, out var orderType) &&
            decimal.TryParse(args[2], out var amount))
        {
            consoleOutput.WriteLine($"Running Console App: {orderType} {amount} BTC", ConsoleColor.Green);
            await Program.RunConsoleAppAsync(orderType, amount);
        }
        else
        {
            consoleOutput.WriteLine("Invalid arguments. Usage: -console [buy|sell] [amount]", ConsoleColor.Red);
        }
    }
}