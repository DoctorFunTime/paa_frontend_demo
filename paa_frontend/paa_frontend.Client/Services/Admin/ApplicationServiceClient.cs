using System.Net.Http.Json;
using paa_frontend.Shared.Models;

namespace paa_frontend.Client.Services.Admin;

public class ApplicationServiceClient(HttpClient httpClient)
{
    public async Task<List<StudentApplication>> GetApplicationsAsync() =>
        await httpClient.GetFromJsonAsync<List<StudentApplication>>("/api/v1/application/get/all")
        ?? throw new InvalidOperationException("The application service returned an empty response.");

    public async Task SetDecisionAsync(long id, bool accept)
    {
        using var response = await httpClient.PatchAsync(
            $"/api/v1/application/{(accept ? "accept" : "reject")}/{id}", null);
        response.EnsureSuccessStatusCode();
    }
}
