﻿using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDTO _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;
        public OrderAPIController(IMapper mapper, AppDbContext db, IProductService productService)
        {
            _response = new ResponseDTO();
            _mapper = mapper;
            _db = db;
            _productService = productService;
        }
        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder(CartDTO cartDTO) {
            try {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = StaticDetails.Status_Pending;
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDTO.CartDetails);
       
                OrderHeader orderCreated =_db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDTO)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDTO.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDTO; 
            }
            catch(Exception ex) {
                _response.IsSuccess=false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO) 
        {
            try {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    
                };
                
                var DiscountsList = new List<SessionDiscountOptions>() {
                
                    new SessionDiscountOptions()
                    {
                        Coupon=stripeRequestDTO.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDTO.OrderHeader.OrderDetails) 
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),//20.99 => 2099 thats how stripe wants it
                            Currency = "inr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }

                        },
                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }
                if (stripeRequestDTO.OrderHeader.Discount > 0) 
                {
                    options.Discounts = DiscountsList;
                }
                

                var service = new SessionService();
                Session session = service.Create(options);//this creates the session, this session has the session id and the url
                stripeRequestDTO.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeaders.First(u=>u.OrderHeaderId==stripeRequestDTO.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                await _db.SaveChangesAsync();
                _response.Result = stripeRequestDTO;
            }
            catch(Exception ex) {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDTO> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);//this creates the session, this session has the session id and the url

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status.Equals("succeeded"))
                {
                    //payment was successful
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = StaticDetails.Status_Approved;
                    await _db.SaveChangesAsync();
                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
                else if (paymentIntent.Status.Equals("requires_action")) 
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = StaticDetails.Status_Pending;
                    await _db.SaveChangesAsync();
                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
                else //if (paymentIntent.Status.Equals("canceled")) 
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = StaticDetails.Status_Cancelled;
                    await _db.SaveChangesAsync();
                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
