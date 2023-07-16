using System.Reflection.Metadata.Ecma335;

namespace Mango.Web.Models
{
    public class CartDTO
    {
        public CartHeaderDTO CartHeader { get; set; }
        public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }
        
    }
}
