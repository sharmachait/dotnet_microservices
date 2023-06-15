using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientfactory;
        public CouponService(IHttpClientFactory httpClientfactory)
        {
            _httpClientfactory = httpClientfactory;
        }
        public async Task<CouponDTO> GetCouponByCode(string couponCode)
        {
            var client = _httpClientfactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/CouponAPI/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(resp.Result));
            }
            return new CouponDTO();
        }
    }
}
