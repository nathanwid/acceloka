using Acceloka.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _service;

        public TicketController(TicketService service)
        {
            _service = service;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> GetAvailableTickets(
            [FromQuery] string? categoryName,
            [FromQuery] string? ticketCode,
            [FromQuery] string? ticketName,
            [FromQuery] int? maxPrice,
            [FromQuery] DateTime? minEventDate,
            [FromQuery] DateTime? maxEventDate,
            [FromQuery] string? orderBy,
            [FromQuery] string? orderState,
            [FromQuery] int page = 1)
        {
            var (tickets, totalTickets) = await _service.GetAvailableTickets(categoryName, ticketCode, ticketName, maxPrice, minEventDate, maxEventDate, orderBy, orderState, page, 10);

            return Ok(new { tickets, totalTickets });
        }
    }
}
