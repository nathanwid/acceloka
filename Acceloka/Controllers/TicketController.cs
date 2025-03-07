using Acceloka.Commands;
using Acceloka.Validators;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ISender _sender;

        public TicketController(ISender sender)
        {
            _sender = sender;
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
            [FromQuery] int? page)
        {
            var command = new GetAvailableTicketsCommand(
                categoryName, ticketCode, ticketName, maxPrice, minEventDate, maxEventDate, orderBy, orderState, page);

            var (tickets, totalTickets) = await _sender.Send(command);

            return Ok(new { tickets, totalTickets });
        }
    }
}
