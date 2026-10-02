using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace paa_frontend.Client.Services.Auth
{
    public class CustomAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;
        // We inject the IJSRuntime to talk to browser storage
        public CustomAuthorizationMessageHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. Grab the token string from Local Storage 
            var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "primeappDemo.authToken");
            // 2. If we found a token, attach it to the request as a Bearer token
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            // 3. Continue sending the request down the pipeline to the Server Gateway
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
