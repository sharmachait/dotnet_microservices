﻿using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    
    public class CouponController : Controller
    {

        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
  /*      [Authorize(Roles = "ADMIN")]*/
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO>? list = new();
            ResponseDTO? response = await _couponService.GetAllCouponsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
            }
            else 
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }
/*        [Authorize(Roles = StaticDetails.RoleAdmin)]*/
        public async Task<IActionResult> CouponCreate()
        {

            return View();
        }
/*        [Authorize(Roles = StaticDetails.RoleAdmin)]*/
        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDTO model)
        {
            if (ModelState.IsValid) {
                ResponseDTO? response = await _couponService.CreateCouponsAsync(model);

                if (response != null && response.IsSuccess)
                {
					TempData["success"] = "Coupon Created";
					return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }
/*        [Authorize(Roles = StaticDetails.RoleAdmin)]*/
        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseDTO? response = await _couponService.GetCouponByIdAsync(couponId);

            if (response != null && response.IsSuccess)
            {
                CouponDTO? model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
/*        [Authorize(Roles = StaticDetails.RoleAdmin)]*/
        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDTO model)
        {
            ResponseDTO? response = await _couponService.DeleteCouponsAsync(model.CouponId);

            if (response != null && response.IsSuccess)
            {
				TempData["success"] = "Coupon Deleted";
				return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(model);
        }
    }
}
