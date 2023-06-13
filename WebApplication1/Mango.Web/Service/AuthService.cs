using Mango.Web.Models;
using Mango.Web.Service.IService;
using static Mango.Web.Utility.StaticDetails;

namespace Mango.Web.Service
{
	public class AuthService : IAuthService
	{
		private readonly IBaseService _baseService;
		public AuthService(IBaseService baseService)
		{
			_baseService = baseService;
		}
		public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO)
		{
			return await _baseService.SendAsync(new RequestDTO()
			{
				ApiType = ApiType.POST,
				Data = registrationRequestDTO,
				Url = AuthAPIBase + "/api/auth/AssignRole"
			});
		}

		public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
		{
			return await _baseService.SendAsync(new RequestDTO()
			{
				ApiType = ApiType.POST,
				Data = loginRequestDTO,
				Url = AuthAPIBase + "/api/auth/login"
			});
		}

		public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
		{
			return await _baseService.SendAsync(new RequestDTO()
			{
				ApiType = ApiType.POST,
				Data = registrationRequestDTO,
				Url = AuthAPIBase + "/api/auth/register"
			});
		}
	}
}
