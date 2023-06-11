using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
	public class AuthService : IAuthService
	{
		private readonly AppDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
			_db = db;
			_userManager = userManager;
			_roleManager = roleManager;
        }
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
			bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
			if (user == null || isValid == false) {
				return new LoginResponseDTO() { User=null,Token="" };
			}

			UserDTO userDTO = new() {
				Email=user.Email,
				ID=user.Id,
				Name=user.Name,
				PhoneNumber=user.PhoneNumber
			};

			LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
			{
				User=userDTO,
				Token=""
			};

			return loginResponseDTO;
		}

		public async Task<string> Register(RegistrationRequestDTO registrationRequestDTO)
		{
			ApplicationUser user = new()
			{

				UserName = registrationRequestDTO.Email,
				Email = registrationRequestDTO.Email,
				NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
				Name = registrationRequestDTO.Name,
				PhoneNumber = registrationRequestDTO.PhoneNumber
			};

			try 
			{
				var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
				if (result.Succeeded)
				{
					var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDTO.Email);
					UserDTO userDTO = new()
					{
						Email = userToReturn.Email,
						ID = userToReturn.Id,
						Name = userToReturn.Name,
						PhoneNumber = userToReturn.PhoneNumber
					};
					return "";
				}
				else {
					return result.Errors.FirstOrDefault().Description;
				}
			}
			catch (Exception ex)
			{ 
				
			}
			return "Some Error Encountered";
		}
	}
}
