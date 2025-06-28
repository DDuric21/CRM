using Models.DTO;

namespace Models.Responses
{
    public class GetRolePermissionsRS : ResponseBase
    {
        public RolePermissions RolePermissions { get; set; }
    }
}
