using System.Text.Json;

namespace MetaExchange.Console.FileReader;


/// <summary>
/// Reads order book data from a JSON file.
/// </summary>
public class OrderBookReader : IOrderBookReader
{
    /// <summary>
    /// JSON serializer options that ignore case for property names.
    /// </summary>
    private static readonly JsonSerializerOptions JsonSerializerOption = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    /// <summary>
    /// Asynchronously reads the order book data from the specified file.
    /// </summary>
    /// <param name="filePath">The file path of the JSON file containing the order book data.</param>
    /// <returns>A list of <see cref="OrderBook"/> objects.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified file is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file content cannot be parsed into a list of order books.</exception>
    public async Task<List<OrderBook>> ReadOrderBookFromFileAsync(string filePath)
    {
        // Check if the file exists at the specified path
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        // Read the JSON content from the file asynchronously
        var json = await File.ReadAllTextAsync(filePath);

        // Deserialize the JSON content into a list of OrderBook objects
        return JsonSerializer.Deserialize<List<OrderBook>>(json, JsonSerializerOption)
               ?? throw new InvalidOperationException("Failed to parse order books.");
    }

    /// <summary>
    /// Writes the updated list of order books to the JSON file asynchronously.
    /// </summary>
    /// <param name="orderBooks">The updated list of order books to be written to the file.</param>
    public async Task WriteOrderBooksToFileAsync(List<OrderBook> orderBooks, string filePath)
    {
        var json = JsonSerializer.Serialize(orderBooks, JsonSerializerOption);
        await File.WriteAllTextAsync(filePath, json);
    }
}