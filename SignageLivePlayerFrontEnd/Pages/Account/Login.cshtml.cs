using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignageLivePlayerFrontEnd.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.Text;

namespace SignageLivePlayerFrontEnd.Pages.Account
{
    [BindProperties]
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public Credentials? Credentials { get; set; }

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

            var response = await client.PostAsync("/api/auth/token", model);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(token))
            {
                HttpContext.Response.Cookies.Append("jwtToken", token);
                return RedirectToPage("/Index");
            }

            // Authentication failed
            return Page();
        }
    }
}
