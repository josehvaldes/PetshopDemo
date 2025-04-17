using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Requests;
using PetShopAPI.Auth;
using PetShopAPI.Extensions;
using PetShopAPI.Validators;

namespace PetShopAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize()]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;
        private readonly IValidator<ProductRequest> _productRequestValidator;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger,
            IValidator<ProductRequest> productRequestValidator) 
        {
            _logger = logger;
            _productService = productService;
            _productRequestValidator = productRequestValidator;
        }

        [HttpGet("{domain}/{type}")]
        public async Task<IActionResult> GetProducts(string domain, string type, [FromQuery] bool availablesOnly) 
        {
            try 
            {
                _logger.LogInformation($"Request Get Products. Domain: {domain}, type: {type}. AvailablesOnly: {availablesOnly}");
                if (availablesOnly)
                {
                    var list = await _productService.RetrieveAvailablesList(domain, type);
                    return Ok(list);
                }
                else 
                {
                    var list = await _productService.RetrieveAllList(domain, type);
                    return Ok(list);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected internal error: {e.Message}");
                return StatusCode(500, new { Error = new[] { e.Message }, Code = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpPost()]
        public async Task<IActionResult> CreateProduct(ProductRequest request) 
        {
            try
            {
                var validator = await _productRequestValidator.ValidateAsync(request);

                if (!validator.IsValid)
                {
                    validator.AddToModelState(ModelState);
                    return UnprocessableEntity(ModelState);
                }

                var response = await _productService.Create(request);
                if (response != null)
                {
                    return Ok(new { Guid = response.guid });
                }
                else 
                {
                    _logger.LogWarning($"Product not created. Review logs for details {request.Name}, {request.Domain}");
                    return BadRequest(new { Error = new[] { $"Product not created. Review logs for details {request.Name}, {request.Domain}" } });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected internal error: {e.Message}");
                return StatusCode(500, new { Error = new[] { e.Message }, Code = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpPut("{domain}/{name}")]
        public async Task<IActionResult> UpdateProduct(string domain, string name, [FromBody] ProductRequest request)
        {
            try
            {
                request.Domain = domain;
                request.Name = name;

                var validator = await _productRequestValidator.ValidateAsync(request);

                if (!validator.IsValid)
                {
                    validator.AddToModelState(ModelState);
                    return UnprocessableEntity(ModelState);
                }

                var response = await _productService.Update(request);
                if (response)
                {
                    return Ok(new { status = "Updated" });
                }
                else
                {
                    _logger.LogWarning($"Product not updated. Review logs for details {request.Name}, {request.Domain}");
                    return BadRequest(new { Error = new[] { $"Product not updated. Review logs for details {request.Name}, {request.Domain}" } });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected internal error: {e.Message}");
                return StatusCode(500, new { Error = new[] { e.Message }, Code = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpDelete("{domain}/{name}")]
        public async Task<IActionResult> DeleteProduct(string domain, string name)
        {
            try
            {
                var response = await _productService.Delete(domain, name);
                if (response)
                {
                    return Ok(new { status = "deleted" });
                }
                else
                {
                    _logger.LogWarning($"Product {domain}/{name} was not found or could not be deleted");
                    return BadRequest(new { Error = new[] { $"Product {domain}/{name} was not found or could not be deleted" } });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected internal error: {e.Message}");
                return StatusCode(500, new { Error = new[] { e.Message }, Code = StatusCodes.Status500InternalServerError });
            }
        }
    }
}
