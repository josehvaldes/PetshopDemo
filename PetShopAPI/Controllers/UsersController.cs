using Asp.Versioning;
using Azure;
using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetShop.Application.Auth;
using PetShop.Application.Interfaces;
using PetShopAPI.Auth;
using PetShopAPI.Extensions;
using PetShopAPI.Models;
using PetShop.Domain.Entities;

namespace PetShopAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserAuthentication _authUser;
        private readonly ILogger<UsersController> _logger;
        private readonly IPasswdHasher _passwdHasher;
        private readonly IUserService _userService;
        private readonly IValidator<AuthenticationRequest> _authRequestValidator;
        private readonly IValidator<AddUserRequest> _addUserRequestValidator;

        public UsersController(ILogger<UsersController> logger, IUserAuthentication authUser, IPasswdHasher passwdHasher, 
            IUserService userService, 
            IValidator<AuthenticationRequest> authRequestValidator,
            IValidator<AddUserRequest> addUserRequestValidator) 
        {
            _authUser = authUser;
            _logger = logger;
            _passwdHasher = passwdHasher;
            _userService = userService;
            _authRequestValidator = authRequestValidator;
            _addUserRequestValidator = addUserRequestValidator;
        }

        [HttpPost()]
        [Authorize(Role.Administrator)]
        public async Task<IActionResult> CreateUser(AddUserRequest request) 
        {
            try 
            {
                var validator = await _addUserRequestValidator.ValidateAsync(request);

                if (!validator.IsValid)
                {
                    validator.AddToModelState(ModelState);
                    return UnprocessableEntity(ModelState);
                }

                var hash = _passwdHasher.HashPassword(new AuthUser(){}, request.Password );

                var userEnitity = new User() {
                    username= request.Username,
                    domain = request.Domain,
                    email = request.Email??string.Empty,
                    hash = hash,
                };
                var response = await _userService.Create(userEnitity);

                if (response != null)
                {
                    return Ok(new { Guid = response.guid, message = "created" });
                }
                else 
                {
                    return BadRequest(new { Error = new [] { "user not created. review logs for more details" } });
                }
                
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new []{ ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpDelete("{domain}/{username}")]
        [Authorize(Role.Administrator)]
        public async Task<IActionResult> DeleteUser(string domain, string username) 
        {
            try
            {
                var response = await _userService.Delete(domain, username);

                if (response.IsCompleted())
                {
                    return Ok(new { message = "deleted" });
                }
                else
                {
                    return BadRequest(new { Error = response.Messages });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpGet("{domain}/{username}")]
        [Authorize(Role.Administrator)]
        public async Task<IActionResult> RetrieveUser(string domain, string username)
        {
            try
            {
                var response = await _userService.Retrieve(domain, username);

                if (response!=null)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(new { Error = new[] { $" User {domain}/{username} not found." } });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthenticationRequest request) 
        {
            _logger.LogInformation($"Login request: [{request.ToString()}]");

            var validator = await _authRequestValidator.ValidateAsync(request);

            if (!validator.IsValid) 
            {
                validator.AddToModelState(ModelState);
                return UnprocessableEntity(ModelState);
            }

            try 
            {
                var response = await _authUser.Authenticate(request);
                if (response == null)
                {
                    _logger.LogWarning($"Failed login request : [{request.Username}]");
                    return BadRequest(new { Error = new[] { $"Failed login attemp: {request.Username}" }  });
                }

                return Ok(response);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }

    }
}
