namespace Models.DTO
{
    public class UserRoleDTO
    {
        public string RoleName { get; set; }

        public IEnumerable<string> Permissions { get; set; }
    }
}
