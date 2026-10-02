using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace paa_frontend.Client.Services.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;

        public CustomAuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // 1. Blazor calls this constantly to figure out who is logged in
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Go into the browser's storage and look for "primeappDemo.authToken".
                // We wrap this in a specific try/catch because during pre-rendering,
                // JSInterop is not available, which will crash the app silently.
                string? token = null;
                try
                {
                    token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "primeappDemo.authToken");
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        token = await MigrateLegacySessionAsync();
                    }
                }
                catch (JSException)
                {
                    // JS not available yet, just return Not Logged In for now
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                catch (InvalidOperationException)
                {
                    // Same as above, handles pre-rendering errors
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // If there's no token, return an empty user (Not Logged In)
                if (string.IsNullOrWhiteSpace(token))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Check token expiry before trusting it
                if (IsTokenExpired(token))
                {
                    // Token has expired; clear this tab's auth data.
                    try
                    {
                        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.authToken");
                        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.refreshToken");
                        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.authUsername");
                        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.tokenExpiresIn");
                        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.userId");
                        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.isDefaultPassword");
                        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "primeappDemo.defaultOldPassword");
                    }
                    catch { /* JS may not be available — safe to swallow */ }
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // If there IS a valid token, crack it open, pull out the roles,
                // and give Blazor a loaded ClaimsPrincipal (Logged In User)
                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch (Exception)
            {
                Console.WriteLine($"[AUTH STATE ERROR]");
                // If anything crashes, fail safe: log them out
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        private async Task<string?> MigrateLegacySessionAsync()
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "primeappDemo.authToken");
            if (string.IsNullOrWhiteSpace(token)) return null;

            foreach (var key in new[] { "primeappDemo.authToken", "primeappDemo.refreshToken", "primeappDemo.authUsername", "primeappDemo.tokenExpiresIn", "primeappDemo.userId", "primeappDemo.isDefaultPassword", "primeappDemo.defaultOldPassword" })
            {
                var value = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
                if (value is not null)
                    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, value);
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            }

            return token;
        }

        // 2. This is the "shout" method your AuthService calls when they login/logout
        public void NotifyUserAuthentication(string? token)
        {
            // We tell Blazor: "Hey! The state just changed, re-evaluate GetAuthenticationStateAsync!"
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        // --- Helper to decode the JWT string into Claims (Roles, Username, etc) ---
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            try
            {
                var payload = jwt.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                if (keyValuePairs != null)
                {

                    // Map "sub" to ClaimTypes.Name for standard identification
                    if (keyValuePairs.TryGetValue("sub", out var sub))
                    {
                        claims.Add(new Claim(ClaimTypes.Name, sub.ToString()!));
                    }

                    // Map "roles" or "role" (any casing) to ClaimTypes.Role
                    // Spring Security commonly uses these keys:
                    var roleKeys = new[] {
                        "roles", "role",
                        "authorities",  // Spring Security default
                        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                    };
                    bool foundRoles = false;
                    foreach (var key in roleKeys)
                    {
                        if (keyValuePairs.TryGetValue(key, out var rolesValue))
                        {
                            AddRoles(claims, rolesValue);
                            foundRoles = true;
                        }
                    }
                    if (!foundRoles)
                    {
                        // No roles found
                    }

                    // Add all other claims as-is
                    var skipKeys = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
                        { "sub", "roles", "role", "authorities" };
                    foreach (var kvp in keyValuePairs)
                    {
                        if (skipKeys.Contains(kvp.Key)) continue;
                        claims.Add(new Claim(kvp.Key, kvp.Value.ToString()!));
                    }
                }
            }
            catch (Exception)
            {
            }
            return claims;
        }

        private void AddRoles(List<Claim> claims, object roles)
        {
            if (roles is JsonElement element)
            {
                if (element.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in element.EnumerateArray())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item.ToString().ToUpper())); // Ensure Upper for match
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, element.ToString().ToUpper()));
                }
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!.ToUpper()));
            }
        }

        private static bool IsTokenExpired(string jwt)
        {
            try
            {
                var payload = jwt.Split('.')[1];
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }
                var jsonBytes = Convert.FromBase64String(payload);
                using var doc = JsonDocument.Parse(jsonBytes);
                if (doc.RootElement.TryGetProperty("exp", out var expProp))
                {
                    var exp = expProp.GetInt64();
                    var expiry = DateTimeOffset.FromUnixTimeSeconds(exp);
                    return expiry.UtcDateTime < DateTime.UtcNow;
                }
                // No exp claim — treat as never expires (safe default)
                return false;
            }
            catch
            {
                // Malformed token — treat as expired
                return true;
            }
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
