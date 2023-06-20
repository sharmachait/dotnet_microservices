using Mango.MessageBus;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Mango.Services.AuthAPI.Controllers
{

	
	[Route("api/auth")]
	[ApiController]
	public class AuthAPIController : ControllerBase
	{
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
		protected ResponseDTO _response;

        public AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            _configuration = configuration;
            _messageBus = messageBus;
            _authService = authService;
			_response = new();
        }

        [HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model) {
			var errorMessage = await _authService.Register(model);
			if (!string.IsNullOrEmpty(errorMessage)) {
				_response.IsSuccess = false;
				_response.Message=errorMessage;
				return BadRequest(_response);

			}
			await EmailUserRequest(model.Email);
			return Ok(_response);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
		{
			var loginResponseDTO = await _authService.Login(model);
			if (loginResponseDTO.User == null) {
				_response.IsSuccess = false;
				_response.Message = "Invalid Credentials";
				return BadRequest(_response);
			}
			_response.Result = loginResponseDTO;
			return Ok(_response);
		}

		[HttpPost("AssignRole")]
		public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO model)
		{
			var assigneRoleSuccess = await _authService.AssignRole(model.Email,model.Role.ToUpper());
			if (!assigneRoleSuccess)
			{
				_response.IsSuccess = false;
				_response.Message = "Error while assigning role";
				return BadRequest(_response);
			}
			return Ok(_response);
		}

        [HttpPost("EmailUserRequest")]
        public async Task<object> EmailUserRequest(string email)
        {
            try
            {
                await _messageBus.PublishMessage(email, _configuration.GetValue<string>("TopicAndQueueName:EmailUserQueue"));
                _response.Result = true;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }

            return _response;
        }

    }
}
