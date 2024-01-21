using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignageLivePlayerFrontEnd.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json.Serialization;

namespace SignageLivePlayerFrontEnd.Pages
{
    public class EditPlayerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public Player? Player { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public EditPlayerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("sl_client");

            var message = PrepareRequestMessage(HttpMethod.Get, $"https://localhost:7292/api/players/{id}");

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(message);
                response.EnsureSuccessStatusCode();
                var players = await response.Content.ReadAsStringAsync();

                Player = JsonSerializer.Deserialize<Player>(players, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });

                // In this case, getting the player was successful
                return Page();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode.HasValue)
                {
                    switch (ex.StatusCode)
                    {
                        case HttpStatusCode.Forbidden:
                            {
                                return RedirectToPage("/Account/Forbidden");
                            }
                        case HttpStatusCode.InternalServerError:
                            {
                                ErrorMessage = "Something went wrong, please contact technical support for assistance.";
                                return RedirectToPage();
                            }
                    }
                }
            }

            return RedirectToPage("/Account/Login");
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("sl_client");

            var model = new StringContent(
                JsonSerializer.Serialize(Player),
                Encoding.UTF8, Application.Json);

            var message = PrepareRequestMessage(HttpMethod.Put, $"https://localhost:7292/api/players/{id}", model);

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
                        case HttpStatusCode.BadRequest:
                            {
                                ErrorMessage = "Invalid input";
                                return RedirectToPage();
                            }
                        case HttpStatusCode.Unauthorized:
                            {
                                return RedirectToPage("/Account/Login");
                            }
                        case HttpStatusCode.Forbidden:
                            {
                                return RedirectToPage("/Account/Forbidden");
                            }
                        case HttpStatusCode.InternalServerError:
                            {
                                ErrorMessage = "Something went wrong, please contact technical support for assistance.";
                                return RedirectToPage();
                            }
                    }
                }
            }

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

        private HttpRequestMessage PrepareRequestMessage(HttpMethod method, string uri)
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

            return message;
        }
    }
}
