using MetaExchange.Console.Output;

namespace MetaExchange.Console.CommandHandlers;

public interface ICommandHandler
{
    Task ExecuteAsync(string[] args, IConsoleOutput output);
}