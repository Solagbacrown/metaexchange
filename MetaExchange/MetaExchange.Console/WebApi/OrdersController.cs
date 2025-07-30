using MetaExchange.Console.Engine;
using MetaExchange.Console.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.Console.WebApi;

/// <summary>
/// Controller for handling order-related operations.
/// This controller provides an endpoint to process buy and sell orders.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OrdersController"/> class.
/// </remarks>
/// <param name="orderProcessor">The order processor to handle the order processing logic.</param>
[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderProcessor orderProcessor) : ControllerBase
{
    private readonly IOrderProcessor _orderProcessor = orderProcessor;

    /// <summary>
    /// Handles HTTP POST requests to place a new order.
    /// Processes the provided <see cref="OrderRequest"/> and returns the result.
    /// </summary>
    /// <param name="request">The order request, including the order type and the amount of BTC to buy or sell.</param>
    /// <returns>An IActionResult containing the order response, or a BadRequest if the model is invalid.</returns>
    /// <response code="200">Returns the order response with details of the processed order.</response>
    /// <response code="400">If the model state is invalid, returns a BadRequest with the validation errors.</response>
    [HttpPost]
    public async Task<IActionResult> Order([FromBody] OrderRequest request)
    {
        // Check if the provided model is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Process the order request
        var orderResponse = await _orderProcessor.ProcessAsync(request);

        // Return the response with order details
        return Ok(orderResponse);
    }
}