using System.Net.Http.Json;
using paa_frontend.Shared.Models;

namespace paa_frontend.Client.Services;

public sealed class PublicApplicationServiceClient(HttpClient httpClient)
{
    public async Task<StudentApplication> SubmitAsync(StudentApplicationSubmission submission)
    {
        using var response = await httpClient.PostAsJsonAsync("/api/v1/application/apply", submission);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StudentApplication>()
            ?? throw new InvalidOperationException("The application service returned an empty response.");
    }
}
