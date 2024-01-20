using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignageLivePlayerFrontEnd.Models;

namespace SignageLivePlayerFrontEnd.Pages.Account
{
    public class RegistrationSuccessModel : PageModel
    {
        public IActionResult OnPost()
        {
            return RedirectToPage("/Account/Login");
        }
    }
}
