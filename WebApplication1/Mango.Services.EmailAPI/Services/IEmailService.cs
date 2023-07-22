using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.DTO;

namespace Mango.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDTO cartDTO);
        Task EmailUserRegister(string email);
        Task LogOrderPlaced(RewardsMessage rewardsDTO);
    }
}
