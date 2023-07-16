using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps() {
            var mappingConfig = new MapperConfiguration(
                config =>
                {
                    config.CreateMap<OrderHeaderDTO, CartHeaderDTO>()
                    .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();
                    //reverser map handle the reversermapping of the map we gave
                    config.CreateMap<CartDetailsDTO,OrderDetailsDTO>()
                    .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                    .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));
                    // we dont need a reverse map for name and price
                    config.CreateMap<OrderDetailsDTO, CartDetailsDTO>();
                    config.CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
                    config.CreateMap<OrderDetailsDTO, OrderDetails>().ReverseMap();
                }
                );
            return mappingConfig;
        }
    }
}
