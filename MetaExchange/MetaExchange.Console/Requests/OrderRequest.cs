using MetaExchange.Console.Models;
using System.ComponentModel.DataAnnotations;

namespace MetaExchange.Console.Requests;

public class OrderRequest
{
    [Required(ErrorMessage = "Order type is required.")]
    [EnumDataType(typeof(OrderType), ErrorMessage = "Invalid order type.")]
    public required OrderType OrderType { get; init; }

    [Range(0.00000001, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; init; }
}
