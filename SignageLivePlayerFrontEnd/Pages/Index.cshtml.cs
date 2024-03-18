using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignageLivePlayerFrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string UserName { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (User.Identity is not null && !User.Identity.IsAuthenticated)
                return RedirectToPage("/Account/Login");
            else
                UserName = User.Identity.Name;

            return Page();
        }
    }
}
