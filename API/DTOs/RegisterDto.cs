using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        // Both of the fields will return Bad Request 400 if user provide empty fields.
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
