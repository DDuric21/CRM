namespace Models.DTO
{
    public class RolePermissions
    {
        public string RoleName { get; set; }

        public Dictionary<string, bool> Permissions { get; set; }

        public RolePermissions()
        {
            Permissions = new Dictionary<string, bool>();
        }

        public RolePermissions(string roleName, Dictionary<string, bool> permissions)
        {
            RoleName = roleName;
            Permissions = permissions;
        }
    }
}
