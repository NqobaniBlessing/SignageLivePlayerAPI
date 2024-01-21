using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignageLivePlayerFrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public string? UserName { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (!HttpContext.Session.Keys.Contains("Token"))
            {
                return RedirectToPage("/Account/Login");
            }
            
            HttpContext.Session.TryGetValue("email", out var username);
            return Page();
        }
    }
}
