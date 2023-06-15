using System.Reflection.Metadata.Ecma335;

namespace Mango.Services.ShoppingCartAPI.Models.DTO
{
    public class CartDTO
    {
        public CartHeaderDTO CartHeader { get; set; }
        public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }
        /*public CouponDTO Coupon { get; set; }*/
    }
}
