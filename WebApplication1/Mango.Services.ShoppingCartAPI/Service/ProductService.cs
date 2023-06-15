using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientfactory;
        public ProductService(IHttpClientFactory httpClientfactory)
        {
            _httpClientfactory = httpClientfactory;
        }
        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var client = _httpClientfactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/ProductAPI");
            var apiContent =await response.Content.ReadAsStringAsync();
            var resp=JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if (resp.IsSuccess) 
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(Convert.ToString(resp.Result));
            }
            return new List<ProductDTO>();
        }
    }
}
