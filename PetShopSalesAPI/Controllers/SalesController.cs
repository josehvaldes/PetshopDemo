using Asp.Versioning;
using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetShop.Model;
using PetShop.Service;
using PetShopSalesAPI.Auth;
using PetShopSalesAPI.Extensions;

namespace PetShopAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly IValidator<SalesRequest> _salesRequestValidator;
        private readonly ISaleService _saleService;
        public SalesController(ILogger<SalesController> logger, IValidator<SalesRequest> salesRequestValidator, ISaleService saleService) 
        {
            _logger = logger;
            _salesRequestValidator = salesRequestValidator;
            _saleService = saleService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateSale(SalesRequest request) 
        {
            try 
            {
                var validator = await _salesRequestValidator.ValidateAsync(request);

                if (!validator.IsValid)
                {
                    validator.AddToModelState(ModelState);
                    return UnprocessableEntity(ModelState);
                }

                var user = (User?)HttpContext.Items["User"];
                if (user == null)
                {
                    return BadRequest("Authorized user not found");
                }

                request.Username = user?.UserName ?? string.Empty;
                request.Domain = user?.Domain ?? string.Empty;

                var response = await _saleService.Create(request);
                if (response.IsCompleted())
                {
                    return Ok(new { saleId = response.SaleId, status = "Completed" });
                }
                else 
                {
                    return BadRequest(new { Error = response.Messages});
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpGet()]
        public async Task<IActionResult> RetrieveSales([FromQuery]string domain) 
        {
            try
            {
                var sales = await _saleService.RetrieveList(domain);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpGet()]
        public async Task<IActionResult> RecommentSales([FromQuery] string userId)
        {
            await Task.FromResult(0);
            return Ok();
        }
    }
}
