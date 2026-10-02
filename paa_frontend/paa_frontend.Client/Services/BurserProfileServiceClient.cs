using paa_frontend.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static paa_frontend.Shared.Models.BurserProfileModel;

namespace paa_frontend.Client.Services
{
    public class BurserProfileServiceClient
    {
        private readonly HttpClient _httpClient;

        public BurserProfileServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Burser>> GetBursersAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<BurserPage>("/api/v1/burser/get/all");
            return response?.Content ?? new List<Burser>();
        }

        public async Task<Burser> GetBurserAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Burser>($"/api/v1/burser/get/{id}");
        }

        public async Task<HttpResponseMessage> CreateBurserAsync(Burser burser)
        {
            return await _httpClient.PostAsJsonAsync("/api/v1/burser/create", burser);
        }

        // BurserProfileServiceClient
        public async Task<AuthCreationResult?> CreateBurserAndReturnAsync(Burser burser)
        {
            var resp = await _httpClient.PostAsJsonAsync("/api/v1/burser/create", burser);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<AuthCreationResult>();
        }

        public async Task<HttpResponseMessage> UpdateBurserAsync(long id, Burser burser)
        {
            return await _httpClient.PatchAsJsonAsync($"/api/v1/burser/update/{id}", burser);
        }

        public async Task<HttpResponseMessage> DeleteBurserAsync(long id)
        {
            return await _httpClient.DeleteAsync($"/api/v1/burser/delete/{id}");
        }
    }
}
