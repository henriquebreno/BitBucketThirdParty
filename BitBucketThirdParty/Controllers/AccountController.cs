using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace BitBucketThirdParty.Controllers
{
    public class AccountController : Controller
    {

        private readonly IConfiguration _configuration;
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }); // Indicates that we want to use the OpenID Connect scheme for authentication
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        // Method to redirect the user to the Bitbucket authorization screen
        public IActionResult Authorize()
        {
            // Build the Bitbucket authorization URL
            var authorizationUrl = "https://bitbucket.org/site/oauth2/authorize";
            var clientId = _configuration["Bitbucket:ClientId"];// Replace with your Bitbucket Client ID
            var redirectUri = "https://localhost:7269/account/callback"; // Replace with your Redirect URI

            var queryParams = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "response_type", "code" }, // Requests the authorization code
            { "redirect_uri", redirectUri },
            { "scope", "repository" } // Requests permission to read user repositories
        };

            // Build the URL with query parameters
            var urlBuilder = new StringBuilder(authorizationUrl);
            urlBuilder.Append("?");
            foreach (var param in queryParams)
            {
                urlBuilder.Append($"{param.Key}={Uri.EscapeDataString(param.Value)}&");
            }
            var finalUrl = urlBuilder.ToString().TrimEnd('&');

            // Redirect the user to the Bitbucket authorization screen
            return Redirect(finalUrl);
        }

        public async Task<IActionResult> Callback(string code)
        {
            // Exchange the authorization code for an access token
            var tokenUrl = "https://bitbucket.org/site/oauth2/access_token";
            var clientId = _configuration["Bitbucket:ClientId"]; // Replace with your Bitbucket Client ID
            var clientSecret = _configuration["Bitbucket:ClientSecret"]; // Replace with your Bitbucket Client Secret
            var redirectUri = "https://localhost:7269/account/callback"; // Replace with your Redirect URI

            using (var httpClient = new HttpClient())
            {
                var requestParams = new Dictionary<string, string>
     {
         { "grant_type", "authorization_code" },
         { "code", code },
         { "client_id", clientId },
         { "client_secret", clientSecret },
         { "redirect_uri", redirectUri }
     };

                var requestContent = new FormUrlEncodedContent(requestParams);
                var response = await httpClient.PostAsync(tokenUrl, requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Example of how to process the response (you can adjust as needed)
                if (response.IsSuccessStatusCode)
                {
                    // Process the response and store the access token
                    // For example, you can save the token in a session or a database
                    // Here, we just return the response as a string for demonstration purposes
                    ViewData["AccessToken"] = JObject.Parse(responseContent)["access_token"].ToString();

                    // List user repositories
                    var repositories = await ListRepositories(ViewData["AccessToken"].ToString());
                    ViewData["Repositories"] = repositories;

                    return View(repositories);
                }
                else
                {
                    // An error occurred while exchanging the authorization code for the access token
                    return Content("Error obtaining access token");
                }
            }
        }

        // Method to list user repositories
        private async Task<IEnumerable<string>> ListRepositories(string accessToken)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync("https://api.bitbucket.org/2.0/repositories");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var repositories = JObject.Parse(content)["values"];
                return repositories.Select(r => r["full_name"].ToString());
            }
            else
            {
                // Handle error
                return new List<string>();
            }
        }


    }
}
