using Microsoft.AspNetCore.Identity;

namespace Backend_API.Data.Models
{
    public class RolePermission : BaseModel
    {
        //should be equivalent with Models.Authentication.CrmPermissionNames
        public string Name { get; set; }
    }
}
