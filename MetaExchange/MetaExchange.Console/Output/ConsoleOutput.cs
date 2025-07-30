namespace MetaExchange.Console.Output;

public class ConsoleOutput : IConsoleOutput
{
    public void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
    {
        var previousColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = color;
        System.Console.WriteLine(message);
        System.Console.ForegroundColor = previousColor;
    }
}