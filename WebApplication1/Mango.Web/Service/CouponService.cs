using Mango.Web.Models;
using Mango.Web.Service.IService;
using static Mango.Web.Utility.StaticDetails;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> CreateCouponsAsync(CouponDTO couponDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data= couponDTO,
                Url = CouponAPIBase + "/api/CouponAPI/"
            });
        }

        public async Task<ResponseDTO?> DeleteCouponsAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.DELETE,
                Url = CouponAPIBase + "/api/CouponAPI/" + id
            });
        }

        public async Task<ResponseDTO?> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url=CouponAPIBase+ "/api/CouponAPI"
            });
        }

        public async Task<ResponseDTO?> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDTO() { 
                ApiType= ApiType.GET,
                Url=CouponAPIBase+ "/api/CouponAPI/GetByCode/" + couponCode
            });
        }

        public async Task<ResponseDTO?> GetCouponByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = CouponAPIBase + "/api/CouponAPI/" + id
            });
        }

        public async Task<ResponseDTO?> UpdateCouponsAsync(CouponDTO couponDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.PUT,
                Data = couponDTO,
                Url = CouponAPIBase + "/api/CouponAPI/"
            });
        }
    }
}
