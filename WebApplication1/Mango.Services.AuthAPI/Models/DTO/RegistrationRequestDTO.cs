﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Services.AuthAPI.Models.DTO
{
	public class RegistrationRequestDTO
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
