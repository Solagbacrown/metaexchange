using MetaExchange.Console.Engine;
using MetaExchange.Console.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.Console.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderProcessor orderProcessor) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Order([FromBody] OrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderResponse = await orderProcessor.ProcessAsync(request);
            return Ok(orderResponse);
        }

    }
}