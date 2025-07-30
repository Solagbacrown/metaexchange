namespace MetaExchange.Console.Output;

public interface IConsoleOutput
{
    void WriteLine(string message, ConsoleColor color = ConsoleColor.White);
}