﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignageLivePlayerFrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public string UserName { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            if(!User.Identity.IsAuthenticated)
            {
                RedirectToPage("/Account/Login");
            }

            UserName = HttpContext.User.Identity.Name;
        }
    }
}