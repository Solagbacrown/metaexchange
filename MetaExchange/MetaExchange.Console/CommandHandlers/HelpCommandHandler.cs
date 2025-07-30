using MetaExchange.Console.Output;

namespace MetaExchange.Console.CommandHandlers;

public class HelpCommandHandler : ICommandHandler
{
    public Task ExecuteAsync(string[] args, IConsoleOutput consoleOutput)
    {
        consoleOutput.WriteLine("Usage Instructions:", ConsoleColor.Yellow);
        consoleOutput.WriteLine("-web         Runs the web application", ConsoleColor.Cyan);
        consoleOutput.WriteLine("-console     Runs the console trader", ConsoleColor.Cyan);
        consoleOutput.WriteLine("                Usage: -console [buy|sell] [amount]", ConsoleColor.Cyan);
        return Task.CompletedTask;
    }
}