using System.ComponentModel.DataAnnotations;

namespace Mango.Services.AuthAPI.Models.DTO
{
	public class UserDTO
	{
        public string ID { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

    }
}
