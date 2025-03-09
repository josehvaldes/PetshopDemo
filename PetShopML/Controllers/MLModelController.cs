using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PetShopML.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MLModelController : ControllerBase
    {
        private readonly ILogger<MLModelController> _logger;

        public MLModelController(ILogger<MLModelController> logger) 
        {
            _logger = logger;
        }

        [HttpGet("ping")]
        public async Task<IActionResult> Ping() 
        {
            await Task.FromResult(0);
            _logger.LogWarning("Ping completed");
            return Ok();
        }


        public async Task<IActionResult> Predict() 
        {

            try 
            {
                await Task.FromResult(0);
                return Ok();
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }
    }
}
