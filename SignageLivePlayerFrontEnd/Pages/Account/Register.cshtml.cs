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
        public string ConfirmPassword { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public RegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Initial validation
            if (NewUser.Password != ConfirmPassword)
            {
                ErrorMessage = "Password and Confirm Password do not match.";
                return RedirectToPage();
            }

            var client = _httpClientFactory.CreateClient("sl_client");

            var model = new StringContent(
                JsonSerializer.Serialize(NewUser),
                Encoding.UTF8, Application.Json);

            try
            {
                var response = await client.PostAsync("/api/users", model);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode.HasValue)
                {
                    switch (ex.StatusCode)
                    {
                        case System.Net.HttpStatusCode.BadRequest:
                            {
                                ErrorMessage = "Invalid input";
                                return RedirectToPage();
                            }
                        case System.Net.HttpStatusCode.InternalServerError:
                            {
                                ErrorMessage = "Something went wrong, please contact technical support for assistance.";
                                return RedirectToPage();
                            }
                    }
                }
            }

            // If you get here, registration is successful
            return RedirectToPage("/Account/RegistrationSuccess");
        }
    }
}
