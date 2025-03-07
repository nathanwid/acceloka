using Acceloka.Commands;
using Acceloka.Models.Requests;
using Acceloka.Services;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        private readonly ISender _sender;

        public BookedTicketController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("get-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> GetBookedTickets(int bookedTicketId)
        {
            var result = await _sender.Send(new GetBookedTicketsCommand(bookedTicketId));
            return Ok(result);
        }

        [HttpGet("generate-excel-report/{bookedTicketId}")]
        public async Task<IActionResult> GenerateExcelReport(int bookedTicketId)
        {
            var result = await _sender.Send(new GenerateExcelReportCommand(bookedTicketId));
            return File(result,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "BookedTicketsReport.xlsx");
        }

        [HttpPost("book-ticket")]
        public async Task<IActionResult> BookTickets([FromBody] BookedTicketModel request)
        {
            var result = await _sender.Send(new BookTicketsCommand(request.BookingDate, request.Tickets));
            return Ok(result);
        }

        [HttpPut("edit-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> EditBookedTicket(int bookedTicketId, [FromBody] BookedTicketModel request)
        {
            var result = await _sender.Send(new EditBookedTicketCommand(bookedTicketId, request));
            return Ok(result);
        }

        [HttpDelete("revoke-ticket/{bookedTicketId}/{ticketCode}/{quantity}")]
        public async Task<IActionResult> RevokeTicket(int bookedTicketId, string ticketCode, int quantity)
        {
            var result = await _sender.Send(new RevokeTicketCommand(bookedTicketId, ticketCode, quantity));
            return Ok(result);
        }
    }
}
