namespace Models.DTO
{
    public class RoleDTO
    {
        public string RoleName { get; set; }

        public Dictionary<string, bool> Permissions { get; set; }

        public RoleDTO()
        {
            Permissions = new Dictionary<string, bool>();
        }

        public RoleDTO(string roleName, Dictionary<string, bool> permissions)
        {
            RoleName = roleName;
            Permissions = permissions;
        }
    }
}
