using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {

        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult OrderIndex()
        {
            return View();
        }

        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDTO orderHeader=new OrderHeaderDTO();
            string userId = User.Claims.Where(U => U.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
            }
            if (!User.IsInRole(StaticDetails.RoleAdmin) && userId!=orderHeader.UserId)
            {
                return NotFound();
            }
            return View(orderHeader);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll(string status) 
        {
            IEnumerable<OrderHeaderDTO> list;
            string userId = "";
            if (!User.IsInRole(StaticDetails.RoleAdmin)) 
            {
                userId = User.Claims.Where(U => U.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            ResponseDTO response = _orderService.GetAllOrder(userId).GetAwaiter().GetResult();
            if (response != null && response.IsSuccess) 
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(Convert.ToString(response.Result));
                switch (status)
                {
                    case "approved":
                        list = list.Where(u => u.Status ==StaticDetails.Status_Approved);
                        break;
                    case "readyforpickup":
                        list = list.Where(u => u.Status == StaticDetails.Status_ReadyForPickup);
                        break;
                    case "cancelled":
                        list = list.Where(u => u.Status == StaticDetails.Status_Cancelled);
                        break;
                    default:
                        break;
                }
            }
            else 
            {
                list=new List<OrderHeaderDTO>();
            }
            return Json(new { data = list });
        }
        
        [Authorize]
        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int OrderId) 
        {
            var response = await _orderService.UpdateOrderStatus(OrderId,StaticDetails.Status_ReadyForPickup);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = OrderId });
            }
            return View();
        }

        [Authorize]
        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int OrderId)
        {
            var response = await _orderService.UpdateOrderStatus(OrderId, StaticDetails.Status_Completed);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = OrderId });
            }
            return View();
        }

        [Authorize]
        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int OrderId)
        {
            var response = await _orderService.UpdateOrderStatus(OrderId, StaticDetails.Status_Cancelled);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = OrderId });
            }
            return View();
        }
    }
}
