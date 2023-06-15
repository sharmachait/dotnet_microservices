using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps() {
            var mappingConfig = new MapperConfiguration(
                config =>
                {
                    config.CreateMap<CartHeader, CartHeaderDTO>();
                    config.CreateMap<CartDetails, CartDetailsDTO>();
                    config.CreateMap<CartHeaderDTO, CartHeader>();
                    config.CreateMap<CartDetailsDTO, CartDetails>();
                }
                );
            return mappingConfig;
        }
    }
}
