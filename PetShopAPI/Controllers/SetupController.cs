using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetShop.Service;

namespace PetShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly ILogger<SetupController> _logger;
        private readonly ISetupService _setupService;

        public SetupController(ILogger<SetupController> logger, ISetupService setupService) 
        {
            _logger = logger;
            _setupService = setupService;
        }

        [HttpPost]
        public async Task<IActionResult> Setup() 
        {
            try
            {
                var result = await _setupService.Setup();
                return Ok(new { messages = result.Messages });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }


    }
}
