using System.ComponentModel.DataAnnotations;

namespace Employee.api.DTOs
{
    public class LoginDto
    {
        [Required]
        public string email { get; set; } = string.Empty;

        [Required]
        public string contactNo { get; set; } = string.Empty;
    }
}
