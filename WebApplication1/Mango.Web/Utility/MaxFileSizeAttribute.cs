using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utility
{
	public class MaxFileSizeAttribute:ValidationAttribute
	{
		private readonly int maxFileSize;
        public MaxFileSizeAttribute(int maxSize)
        {
            maxFileSize= maxSize;
        }
        protected override ValidationResult? IsValid(object? value,ValidationContext validationContext)
		{
			var file=value as IFormFile; 
			if (file != null) 
			{
				if (file.Length>(maxFileSize*1024*1024))
				{
					return new ValidationResult("The photo is too large");
				}
			}
			return ValidationResult.Success;
		}
	}
}
