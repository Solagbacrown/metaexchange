﻿using MetaExchange.Console.FileReader;
using MetaExchange.Console.Resources.Path;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaExchange.Console.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderBooksController : ControllerBase
    {
        private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, Constants.Resources, Constants.Data, Constants.OrderBook);
        private readonly IOrderBookReader _orderBookReader;

        public OrderBooksController(IOrderBookReader orderBookReader)
        {
            _orderBookReader = orderBookReader;
        }

        /// <summary>
        /// Upserts (Updates or Inserts) a list of order books in the JSON file.
        /// </summary>
        /// <param name="upsertOrderBooks">The list of order books to be updated or added.</param>
        /// <returns>Returns a status message indicating success or failure.</returns>
        /// 
        [HttpPost("upsert")]
        [SwaggerOperation(Summary = "Upsert a list of order books", Description = "Updates existing order books or inserts new ones into the data store.")]
        [SwaggerResponse(200, "Order books have been upserted successfully.", typeof(List<OrderBook>))]

        public async Task<IActionResult> UpsertOrderBooks([FromBody] List<OrderBook> upsertOrderBooks)
        {
            // Read the existing order books from the JSON file
            var orderBooks = await _orderBookReader.ReadOrderBookFromFileAsync(_filePath);

            foreach (var upsertOrderBook in upsertOrderBooks)
            {
                // Find the existing order book by exchange ID
                var existingOrderBook = orderBooks.FirstOrDefault(ob => ob.Id == upsertOrderBook.Id);

                if (existingOrderBook != null)
                {
                    // Update the existing order book fields
                    existingOrderBook.EurBalance = upsertOrderBook.EurBalance;
                    existingOrderBook.BtcBalance = upsertOrderBook.BtcBalance;
                    existingOrderBook.AcqTime = upsertOrderBook.AcqTime;
                    existingOrderBook.Asks = upsertOrderBook.Asks;
                    existingOrderBook.Bids = upsertOrderBook.Bids;
                }
                else
                {
                    // If the order book doesn't exist, add it to the list
                    orderBooks.Add(upsertOrderBook);
                }
            }

            // Write the updated order books back to the JSON file
            await _orderBookReader.WriteOrderBooksToFileAsync(orderBooks, _filePath);

            return Ok(orderBooks);
        }

        /// <summary>
        /// Clears all the order books data from the JSON file.
        /// </summary>
        /// <returns>A status message indicating success.</returns>
        [HttpDelete("clear")]
        [SwaggerOperation(Summary = "Clears a list of order books", Description = "Clears existing order books in the data store.")]
        [SwaggerResponse(200, "Order books have been upserted successfully.", typeof(List<OrderBook>))]

        public async Task<IActionResult> ClearOrderBooks()
        {
            var emptyOrderBooks = new List<OrderBook>(); // Empty list of order books
            await _orderBookReader.WriteOrderBooksToFileAsync(emptyOrderBooks, _filePath); // Write empty data to the file
            var orderBooks = await _orderBookReader.ReadOrderBookFromFileAsync(_filePath); //Read empty to confirm

            return Ok(orderBooks);
        }
    }
}
