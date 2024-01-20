using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignageLivePlayerFrontEnd.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.Text;

namespace SignageLivePlayerFrontEnd.Pages.Account
{
    [BindProperties]
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public Credentials? NewUser { get; set; }

        public RegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("sl_client");

            var model = new StringContent(
                JsonSerializer.Serialize(NewUser),
                Encoding.UTF8, Application.Json);

            var response = await client.PostAsync("/api/users", model);
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadAsStringAsync();

            TempData["Created User"] = user;

            if (!string.IsNullOrEmpty(user))
            {
                return RedirectToPage("/Account/RegistrationSuccess");
            }

            // Authentication failed
            return Page();
        }
    }
}
