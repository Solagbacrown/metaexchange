namespace MetaExchange.Console.FileReader;

using System.Text.Json;

public class OrderBookReader : IOrderBookReader
{
    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public async Task<List<OrderBook>> ReadOrderBookFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<List<OrderBook>>(json, CaseInsensitiveOptions)
               ?? throw new InvalidOperationException("Failed to parse order books.");

    }
}