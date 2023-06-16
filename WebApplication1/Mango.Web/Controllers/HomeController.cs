using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public HomeController( IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDTO>? list = new();
            ResponseDTO? response = await _productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> ProductDetails(int ProductId)
        {

            ResponseDTO? response = await _productService.GetProductByIdAsync(ProductId);

            if (response != null && response.IsSuccess)
            {
                ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            TempData["error"]= response?.Message;
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDTO productDTO)
        {

            CartDTO cartDTO = new CartDTO()
            {
                CartHeader = new CartHeaderDTO
                {
                    UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDTO cartDetails = new CartDetailsDTO()
            {
                Count = productDTO.Count,
                ProductId = productDTO.ProductId,
            };

            List<CartDetailsDTO> cartDetailsDTOs = new() { cartDetails };
            cartDTO.CartDetails = cartDetailsDTOs;

            ResponseDTO? response = await _cartService.UpsertCartAsync(cartDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item has been added.";

                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = response?.Message;

            return View(productDTO);
        }

        /*        public async Task<IActionResult> Details(ProductDTO model)
                {
                    ResponseDTO? response = await _productService.UpdateProductAsync(model);

                    if (response != null && response.IsSuccess)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    return View(model);
                }*/

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}