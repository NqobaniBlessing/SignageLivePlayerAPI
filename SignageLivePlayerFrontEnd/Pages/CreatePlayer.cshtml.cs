using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignageLivePlayerFrontEnd.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using SignageLivePlayerFrontEnd.Models.DTOs;

namespace SignageLivePlayerFrontEnd.Pages
{
    public class CreatePlayerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public PlayerDTO? Player { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public CreatePlayerModel(IHttpClientFactory httpClientFactory)
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
                JsonSerializer.Serialize(Player),
                Encoding.UTF8, Application.Json);

            var message = PrepareRequestMessage(HttpMethod.Post, $"https://localhost:7292/api/players", model);

            try
            {
                var response = await client.SendAsync(message);
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
                                ErrorMessage = "Invalid Input";
                                return RedirectToPage("/Account/Login");
                            }
                        case System.Net.HttpStatusCode.Unauthorized:
                            {
                                return RedirectToPage("/Account/Login");
                            }
                        case System.Net.HttpStatusCode.Forbidden:
                            {
                                return RedirectToPage("/Account/Forbidden");
                            }
                        case System.Net.HttpStatusCode.InternalServerError:
                            {
                                ErrorMessage = "Something went wrong, please contact technical support for assistance.";
                                return RedirectToPage();
                            }
                    }
                }
            }

            // Player creation was successful
            return RedirectToPage("/Players");
        }

        private HttpRequestMessage PrepareRequestMessage(HttpMethod method, string uri, StringContent model)
        {
            // Extract access token for use in request
            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            string accessToken = string.Empty;

            if (authorizationHeader.ToString().StartsWith("Bearer"))
            {
                accessToken = authorizationHeader.ToString().Substring("Bearer ".Length).Trim().Replace("\"", "");
            }

            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            message.Method = method;
            message.RequestUri = new Uri(uri);
            message.Content = model;

            return message;
        }
    }
}
