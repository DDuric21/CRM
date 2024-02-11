using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;

namespace CRM_UI.Pages
{
    public class LoginModel : PageModel
    {
        public string CustomName { get; set; }
        public void OnGet()
        {
            this.CustomName = "Test";
        }

        public void OnPost(UserDTO userDTO)
        {
            //we are here
            this.CustomName = "DOne";
        }
    }
}
