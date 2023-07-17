using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System.Collections;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;

        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            var data = await LoadCartDTOBasedOnLoggedInUser();
            if (data.CartDetails == null || data.CartHeader == null) {
                TempData["error"] = "Cart is empty.";
                return RedirectToAction("Index", "Home");
            }

            return View(data);
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var data = await LoadCartDTOBasedOnLoggedInUser();
            if (data.CartDetails == null || data.CartHeader == null)
            {
                TempData["error"] = "Cart is empty.";
                return RedirectToAction("Index", "Home");
            }

            return View(data);
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartDTOBasedOnLoggedInUser();
            cart.CartHeader.Phone=cartDTO.CartHeader.Phone;
            cart.CartHeader.Email=cartDTO.CartHeader.Email;
            cart.CartHeader.Name=cartDTO.CartHeader.Name;
            var response = await _orderService.CreateOrder(cart);
            OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
            if (response != null && response.IsSuccess) 
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDTO stripeRequestDTO = new()
                {
                    ApprovedUrl = domain+ "Cart/Confirmation?orderid="+orderHeaderDTO.OrderHeaderId,
                    CancelUrl = domain+"Cart/Checkout",
                    OrderHeader=orderHeaderDTO
                };
                var responseStripe = await _orderService.CreateStripeSession(stripeRequestDTO);
                StripeRequestDTO stripeResponse = JsonConvert.DeserializeObject<StripeRequestDTO>(Convert.ToString(responseStripe.Result));
                Response.Headers.Add("Location", stripeResponse.StripeSessionUrl);
                return new StatusCodeResult(303);//means redirection to another page
            }
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            
            ResponseDTO? response = await _orderService.ValidateStripeSession(orderId);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDTO orderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
                if (orderHeader.Status.Equals(StaticDetails.Status_Approved)) {
                    return View(orderId);
                }
            }
            //redirect to some error page
            return View(orderId);
        }


        public async Task<IActionResult> Remove(int cartDetailsId) 
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated succesfully.";
                return RedirectToAction(nameof(CartIndex));

            }
            /*TempData["success"] = response.Message;*/
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO)
        {
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon Applied.";
                return RedirectToAction(nameof(CartIndex));

            }
            TempData["success"] = response.Message;
            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartDTOBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.EmailCart(cart);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Email will be sent shortly.";
                return RedirectToAction(nameof(CartIndex));

            }
            TempData["success"] = response.Message;
            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
        {
            ResponseDTO? response = await _cartService.RemoveCouponAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon Applied.";
                return RedirectToAction(nameof(CartIndex));

            }
            TempData["success"] = response.Message;
            return RedirectToAction(nameof(CartIndex));
        }

        private async Task<CartDTO> LoadCartDTOBasedOnLoggedInUser() 
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                return cartDTO;
            }

            return new CartDTO();
        }
    }
}
