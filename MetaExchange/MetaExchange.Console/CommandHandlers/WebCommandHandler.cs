using MetaExchange.Console.Output;

namespace MetaExchange.Console.CommandHandlers;

public class WebCommandHandler : ICommandHandler
{
    public async Task ExecuteAsync(string[] args, IConsoleOutput consoleOutput)
    {
        consoleOutput.WriteLine("Starting Web App...", ConsoleColor.Cyan);
        await Program.RunWebApiAsync();
    }
}