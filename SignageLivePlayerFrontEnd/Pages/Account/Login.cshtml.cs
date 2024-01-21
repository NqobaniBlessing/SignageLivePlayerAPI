using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignageLivePlayerFrontEnd.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.Text;
using System.Net;

namespace SignageLivePlayerFrontEnd.Pages.Account
{
    [BindProperties]
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public Credentials? Credentials { get; set; }

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync() 
        {
            var client = _httpClientFactory.CreateClient("sl_client");

            var model = new StringContent(
                JsonSerializer.Serialize(Credentials),
                Encoding.UTF8, Application.Json);

            try
            {
                var response = await client.PostAsync("/api/auth/token", model);
                response.EnsureSuccessStatusCode();

                var token = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    // Store token in session for reuse in subsequent requests
                    HttpContext.Session.SetString("Token", token);
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode.HasValue)
                {
                    switch (ex.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            {
                                ErrorMessage = "Invalid Username or password.";
                                return RedirectToPage();
                            }
                        case HttpStatusCode.InternalServerError:
                            {
                                ErrorMessage = "Something went wrong, please contact technical support";
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
