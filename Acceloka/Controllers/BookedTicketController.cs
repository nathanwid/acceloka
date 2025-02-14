using Acceloka.Models;
using Acceloka.Services;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        private readonly BookedTicketService _service;

        public BookedTicketController(BookedTicketService service)
        {
            _service = service;
        }

        [HttpGet("get-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> GetBookedTickets(int bookedTicketId)
        {
            try
            {
                var result = await _service.GetBookedTickets(bookedTicketId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Type = "https://example.com/errors/not-found",
                    Title = "Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Type = "https://example.com/errors/internal-server-error",
                    Title = "Internal Server Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = HttpContext.Request.Path
                });
            }
        }

        [HttpGet("generate-excel-report")]
        public async Task<IActionResult> GenerateExcelReport()
        {
            try
            {
                var result = await _service.GenerateExcelReport();

                return File(result,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "BookedTicketsReport.xlsx");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Type = "https://example.com/errors/not-found",
                    Title = "Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Type = "https://example.com/errors/internal-server-error",
                    Title = "Internal Server Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = HttpContext.Request.Path
                });
            }
        }


        [HttpPost("book-ticket")]
        public async Task<IActionResult> BookTickets([FromBody] BookedTicketModel request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Type = "https://example.com/errors/validation-error",
                        Title = "Validation Error",
                        Detail = "Data tidak valid",
                        Status = StatusCodes.Status400BadRequest,
                        Instance = HttpContext.Request.Path
                    });
                }

                var result = await _service.BookTickets(request);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://example.com/errors/validation-error",
                    Title = "Validation Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Type = "https://example.com/errors/internal-server-error",
                    Title = "Internal Server Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = HttpContext.Request.Path
                });
            }
        }

        [HttpPut("edit-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> EditBookedTicket(int bookedTicketId, [FromBody] BookedTicketModel request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Type = "https://example.com/errors/validation-error",
                        Title = "Validation Error",
                        Detail = "Data tidak valid",
                        Status = StatusCodes.Status400BadRequest,
                        Instance = HttpContext.Request.Path
                    });
                }

                var result = await _service.EditBookedTicket(bookedTicketId, request);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Type = "https://example.com/errors/not-found",
                    Title = "Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://example.com/errors/validation-error",
                    Title = "Validation Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Type = "https://example.com/errors/internal-server-error",
                    Title = "Internal Server Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = HttpContext.Request.Path
                });
            }
        }

        [HttpDelete("revoke-ticket/{bookedTicketId}/{ticketCode}/{quantity}")]
        public async Task<IActionResult> RevokeTicket(int bookedTicketId, string ticketCode, int quantity)
        {
            try
            {
                var result = await _service.RevokeTicket(bookedTicketId, ticketCode, quantity);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Type = "https://example.com/errors/not-found",
                    Title = "Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://example.com/errors/validation-error",
                    Title = "Validation Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Type = "https://example.com/errors/internal-server-error",
                    Title = "Internal Server Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = HttpContext.Request.Path
                });
            }
        }
    }
}
