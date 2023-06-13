using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mango.Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
			_authService = authService;
        }

		[HttpGet]
        public IActionResult Login()
		{
			LoginRequestDTO loginRequestDTO = new();
			return View(loginRequestDTO);
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

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
