using Mango.Web.Models;
using Mango.Web.Service.IService;
using static Mango.Web.Utility.StaticDetails;

namespace Mango.Web.Service
{
    public class CartService: ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {

            _baseService = baseService;

        }

        public async Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = ShoppingCartAPIBase + "/api/cart/ApplyCoupon"
            });
        }

        public async Task<ResponseDTO?> EmailCart(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = ShoppingCartAPIBase + "/api/cart/EmailCartRequest"
            });
        }

        public async Task<ResponseDTO?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = ShoppingCartAPIBase + $"/api/cart/GetCart/{userId}"
            });
        }

        public async Task<ResponseDTO?> RemoveCouponAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = ShoppingCartAPIBase + "/api/cart/RemoveCoupon"
            });
        }

        public async Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDetailsId,
                Url = ShoppingCartAPIBase + "/api/cart/RemoveCart"
            });
        }

        public async Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDTO,
                Url = ShoppingCartAPIBase + "/api/cart/CartUpsert"
            });
        }
        
    }
}
