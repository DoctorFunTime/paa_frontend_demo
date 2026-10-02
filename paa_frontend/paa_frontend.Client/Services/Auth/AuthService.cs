using System.Net.Http.Json;
using Microsoft.JSInterop;
using System.Text.Json;
using paa_frontend.Shared.Models.Auth;

namespace paa_frontend.Client.Services.Auth
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly CustomAuthStateProvider _authStateProvider;
        private readonly IJSRuntime _jsRuntime;

        // We use a generic HttpClient here that IS NOT intercepted by our JWT handler
        // because we don't have a token to send when we are logging in!
        public AuthService(HttpClient httpClient, CustomAuthStateProvider authStateProvider, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _jsRuntime = jsRuntime;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            try
            {
                var loginRequest = new { username, password };

                // 1. Ask the Gateway (Port 8080) -> Route 1 -> Java Auth Service (Port 8081)
                var response = await _httpClient.PostAsJsonAsync("api/v1/auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var rawBody = await response.Content.ReadAsStringAsync();

                    // 2. Deserialize the full AuthResponse from the backend
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var authResult = JsonSerializer.Deserialize<LoginResponse>(rawBody, options);
                    var token = authResult?.AccessToken;

                    if (!string.IsNullOrEmpty(token))
                    {
                        // Keep the authenticated identity scoped to this browser tab.
                        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "primeappDemo.authToken", token);

                        if (!string.IsNullOrEmpty(authResult?.RefreshToken))
                            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "primeappDemo.refreshToken", authResult.RefreshToken);

                        if (!string.IsNullOrEmpty(authResult?.Username))
                            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "primeappDemo.authUsername", authResult.Username);

                        if (authResult?.ExpiresIn > 0)
                            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "primeappDemo.tokenExpiresIn", authResult.ExpiresIn.ToString());

                        if (authResult?.UserId > 0)
                            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "primeappDemo.userId", authResult.UserId.ToString());

                        // 4. Tell the Blazor app that someone just logged in!
                        _authStateProvider.NotifyUserAuthentication(token);

                        return token; // Return the token so Login.razor can decode the role
                    }
                }
                return null;
            }
            catch (Exception)
            {
                Console.WriteLine("[AUTH ERROR]");
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            // Remove the authenticated identity from this tab.
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.authToken");
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.refreshToken");
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.authUsername");
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.tokenExpiresIn");
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.userId");
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.isDefaultPassword");
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.defaultOldPassword");

            // 2. Notify the state provider
            _authStateProvider.NotifyUserAuthentication(null);
        }

        public async Task<HttpResponseMessage> ChangePasswordAsync(string oldPassword, string newPassword, string username)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/change-password");
            var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "primeappDemo.authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            request.Content = JsonContent.Create(new { username, oldPassword, newPassword });
            return await _httpClient.SendAsync(request);
        }
    }
}
