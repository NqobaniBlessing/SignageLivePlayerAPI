using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignageLivePlayerFrontEnd.Models;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using SignageLivePlayerFrontEnd.Services.Interfaces;

namespace SignageLivePlayerFrontEnd.Pages.Account
{
    [BindProperties]
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISecurityContextService _securityContextService;

        public Credentials? Credentials { get; set; }
        public bool RememberMe { get; set; }

        public LoginModel(IHttpClientFactory httpClientFactory, 
            ISecurityContextService securityContextService)
        {
            _httpClientFactory = httpClientFactory;
            _securityContextService = securityContextService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync() 
        {
            if (!ModelState.IsValid) return Page();

            var client = _httpClientFactory.CreateClient("sl_client");

            try
            {
                var response = await client.PostAsJsonAsync("/api/auth/token", Credentials);
                response.EnsureSuccessStatusCode();

                var token = await response.Content.ReadAsStringAsync();

                // Create a claims principal for the auth cookie
                var principal = _securityContextService.CreateClaimsPrincipal(token);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = RememberMe
                };

                await HttpContext.SignInAsync("Signage_Live", principal, authProperties);

                // Store original string token in session for reuse in subsequent requests
                HttpContext.Session.SetString("access_token", token);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode.HasValue)
                {
                    switch (ex.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            {
                                ModelState.AddModelError("Auth_Err", "Invalid username or password.");
                                return RedirectToPage();
                            }
                        case HttpStatusCode.InternalServerError:
                            {
                                ModelState.AddModelError("Svr_Err", "Something went wrong, please contact technical support");
                                return RedirectToPage();
                            }
                    }
                }
            }

            // Authentication success at this point
            return RedirectToPage("/Index");
        }
    }
}
