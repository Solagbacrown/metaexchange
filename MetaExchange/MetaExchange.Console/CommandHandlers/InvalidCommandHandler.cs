using MetaExchange.Console.Output;

namespace MetaExchange.Console.CommandHandlers;

public class InvalidCommandHandler : ICommandHandler
{
    public Task ExecuteAsync(string[] args, IConsoleOutput consoleOutput)
    {
        consoleOutput.WriteLine("Invalid argument. Use -help to see available options.", ConsoleColor.Red);
        return Task.CompletedTask;
    }
}