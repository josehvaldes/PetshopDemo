using Asp.Versioning;
using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetShop.Model;
using PetShop.Service;
using PetShopAPI.Auth;
using PetShopAPI.Extensions;
using PetShopAPI.Models;
using PetShopAPI.Validators;

namespace PetShopAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientsController> _logger;
        private readonly IValidator<ClientRequest> _clientRequestValidator;
        private readonly IValidator<ClientUpdateRequest> _clientUpdateRequestValidator;
        
        public ClientsController(ILogger<ClientsController> logger, IClientService clientService,
            IValidator<ClientRequest> clientRequestValidator,
            IValidator<ClientUpdateRequest> clientUpdateRequestValidator)
        {
            _logger = logger;
            _clientService = clientService;
            _clientRequestValidator = clientRequestValidator;
            _clientUpdateRequestValidator = clientUpdateRequestValidator;
        }


        [HttpPost]
        public async Task<IActionResult> CreateClient(ClientRequest request)
        {
            try
            {
                var validator = await _clientRequestValidator.ValidateAsync(request);

                if (!validator.IsValid)
                {
                    validator.AddToModelState(ModelState);
                    return UnprocessableEntity(ModelState);
                }

                var response = await _clientService.Create(request);
                if (response != null)
                {
                    return Ok(new { response.guid });
                }
                else
                {
                    _logger.LogWarning($"Client not created. Review logs for details {request.FullName}, {request.TaxNumber}");
                    return BadRequest(new { Error = new[] { $"Client not created. Review logs for details {request.FullName}, {request.TaxNumber}" } });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpGet("{taxnumber}")]
        public async Task<IActionResult> RetrieveClient(string taxnumber)
        {
            try
            {
                var response = await _clientService.Retrieve(taxnumber);
                if (response != null)
                {
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning($"Client {taxnumber} not found.");
                    return BadRequest(new { Error = new[] { $"Client {taxnumber} not found." } });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpPut("{taxnumber}")]
        public async Task<IActionResult> UpdateClient(string taxnumber, [FromBody] ClientUpdateRequest request)
        {
            try
            {
                var validator = await _clientUpdateRequestValidator.ValidateAsync(request);

                if (!validator.IsValid)
                {
                    validator.AddToModelState(ModelState);
                    return UnprocessableEntity(ModelState);
                }

                var entity = new ClientRequest() {
                    FullName = request.Fullname,
                    TaxNumber = taxnumber,
                };
                var response = await _clientService.Update(entity);
                if (response)
                {
                    return Ok(new { status = "updated" });
                }
                else
                {
                    _logger.LogWarning($"Client {taxnumber} was not found or could not be updated.");
                    return BadRequest(new { Error = new[] { $"Client {taxnumber} was not found or could not be updated." } });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }


        [HttpDelete("{taxnumber}")]
        public async Task<IActionResult> DeleteClient(string taxnumber)
        {
            try
            {
                var response = await _clientService.Delete(taxnumber);
                if (response)
                {
                    return Ok(new { taxnumber, status = "deleted" });
                }
                else
                {
                    _logger.LogWarning($"Client {taxnumber} was not found or could not be deleted");
                    return BadRequest(new { Error = new[] { $"Client {taxnumber} was not found or could not be deleted" } });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }
    }
}
