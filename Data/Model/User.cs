using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Data.Model
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        [ForeignKey("UserRoleId")]
        public long UserRoleId { get; set; }
    }
}
