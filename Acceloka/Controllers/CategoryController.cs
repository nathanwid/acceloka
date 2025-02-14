using Acceloka.Models;
using Acceloka.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoryController(CategoryService service)
        {
            _service = service;
        }

        [HttpGet("get-category")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _service.GetCategories();

            return Ok(result);
        }

        [HttpPut("update-category")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryModel request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Type = "https://example.com/errors/validation-error",
                        Title = "Bad Request",
                        Detail = "Data tidak valid",
                        Status = StatusCodes.Status400BadRequest,
                        Instance = HttpContext.Request.Path
                    });
                }

                var result = await _service.UpdateCategory(request);
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
