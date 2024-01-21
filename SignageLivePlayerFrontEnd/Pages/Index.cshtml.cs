using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Http.Headers;
using SignageLivePlayerFrontEnd.Models;
using System.Security.Claims;
using System.Linq;

namespace SignageLivePlayerFrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public string? UserName { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!HttpContext.Session.Keys.Contains("Token"))
            {
                return RedirectToPage("/Account/Login");
            }

            var client = _httpClientFactory.CreateClient("sl_client");

            var message = PrepareRequestMessage(HttpMethod.Get, "https://localhost:7292/api/users/me");

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(message);
                response.EnsureSuccessStatusCode();
                var userJson = await response.Content.ReadAsStringAsync();

                var user = JsonSerializer.Deserialize<CreatedUser>(userJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });

                List<Claim> claims = new List<Claim>();

                foreach (var kvp in user.Claims)
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value));
                }

                var identity = new ClaimsIdentity(claims, "Bearer");
                User.AddIdentity(identity);

                var myClaims = User.Claims.ToList();
                UserName = myClaims.Find(c => c.Type.Equals("email"))?.Value;
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

            return Page();
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
