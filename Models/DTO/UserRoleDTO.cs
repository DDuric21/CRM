namespace Models.DTO
{
    public class UserRoleDTO
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public IEnumerable<string> Permissions { get; set; }
    }
}
