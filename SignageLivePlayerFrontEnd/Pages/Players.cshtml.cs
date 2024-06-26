using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignageLivePlayerFrontEnd.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.Text;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System;

namespace SignageLivePlayerFrontEnd.Pages
{
    public class PlayersModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public List<Player>? Players { get; set; }

        public PlayersModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("sl_client");

            var message = PrepareRequestMessage(HttpMethod.Get, "https://localhost:7292/api/players");

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(message);
                response.EnsureSuccessStatusCode();
                var players = await response.Content.ReadAsStringAsync();

                Players = JsonSerializer.Deserialize<List<Player>>(players, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });

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
                    }
                }
            }

            return RedirectToPage("/Account/Login");
        }

        public async Task<IActionResult> OnGetDeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("sl_client");
            var message = PrepareRequestMessage(HttpMethod.Delete, $"https://localhost:7292/api/players/{id}");

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
                        case HttpStatusCode.Forbidden:
                            {
                                return RedirectToPage("/Account/Forbidden");
                            }
                    }
                }
            }

            return RedirectToPage("/Players");
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
