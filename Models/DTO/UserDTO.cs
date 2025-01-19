using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
    public class UserDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? UserName { get; set; }

        [Required]
        public string UserEmail { get; set; }

        public string? Password { get; set; }

        public string UserRole { get; set; }
    }
}
