using Models.Enums;
using System.ComponentModel.DataAnnotations;
using Resources.Translations;

namespace Models.DTO
{
    public class UserDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required(
           ErrorMessageResourceType = typeof(ValidationMessages),
           ErrorMessageResourceName = nameof(ValidationMessages.UsernameRequired))]
        public string? UserName { get; set; }

        public string UserEmail { get; set; }

        [Required(
           ErrorMessageResourceType = typeof(ValidationMessages),
           ErrorMessageResourceName = nameof(ValidationMessages.PasswordRequired))]
        public string? Password { get; set; }

        public ItemState UserStatus { get; set; }

        public List<UserRoleDTO> UserRoles { get; set; }
    }
}
