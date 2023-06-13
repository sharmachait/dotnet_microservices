using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
	public class RegistrationRequestDTO
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }

        public string? Role { get; set; }
    }
}
