using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;
		private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService,ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
			_authService = authService;
        }

		[HttpGet]
        public IActionResult Login()
		{
			LoginRequestDTO loginRequestDTO = new();
			return View(loginRequestDTO);
		}

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            ResponseDTO responseDTO = await _authService.LoginAsync(model);
            
            if (responseDTO != null && responseDTO.IsSuccess)
            {
                LoginResponseDTO loginResponseDTO = 
                    JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(responseDTO.Result));


                await SignInUser(loginResponseDTO.Token);
                _tokenProvider.SetToken(loginResponseDTO.Token);

                return RedirectToAction("Index", "Home");

            }
            else
            {
                ModelState.AddModelError("CustomError", responseDTO.Message);
                TempData["error"] = responseDTO.Message;
                return View(model);
            }
        }

        private async Task SignInUser(string token) {

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, 
                jwt.Claims.FirstOrDefault(u=>u.Type== JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, 
                jwt.Claims.FirstOrDefault(u=>u.Type== JwtRegisteredClaimNames.Sub).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, 
                jwt.Claims.FirstOrDefault(u=>u.Type== JwtRegisteredClaimNames.Name).Value));


            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));



            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
        }










        [HttpGet]
		public IActionResult Register()
		{
			var roleList = new List<SelectListItem>() {
				new SelectListItem{Text=StaticDetails.RoleAdmin,Value=StaticDetails.RoleAdmin },
				new SelectListItem{Text=StaticDetails.RoleCustomer,Value=StaticDetails.RoleCustomer }
			};
			ViewBag.RoleList = roleList;
			return View();
		}

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO model)
        {
            ResponseDTO responseDTO = await _authService.RegisterAsync(model);
			ResponseDTO assignRole;
			if (responseDTO != null && responseDTO.IsSuccess)
			{
				if (string.IsNullOrEmpty(model.Role))
				{
					model.Role = StaticDetails.RoleCustomer;
				}

				assignRole = await _authService.AssignRoleAsync(model);
				if (assignRole != null && assignRole.IsSuccess)
				{
					TempData["success"] = "Registration Succesful";
					return RedirectToAction(nameof(Login));
				}

			}
			else {
                TempData["error"] = responseDTO.Message;
            }
            var roleList = new List<SelectListItem>() {
                new SelectListItem{Text=StaticDetails.RoleAdmin,Value=StaticDetails.RoleAdmin },
                new SelectListItem{Text=StaticDetails.RoleCustomer,Value=StaticDetails.RoleCustomer }
            };
            ViewBag.RoleList = roleList;
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
