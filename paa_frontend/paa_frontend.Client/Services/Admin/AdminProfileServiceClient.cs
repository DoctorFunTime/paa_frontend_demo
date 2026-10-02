using System.Net.Http;
using System.Net.Http.Json;
using AdminPageResponse = paa_frontend.Shared.Models.AdminProfileModel.AdminPage;
using AdminProfile = paa_frontend.Shared.Models.AdminProfileModel.Admin;

namespace paa_frontend.Client.Services.Admin
{
    public class AdminProfileServiceClient
    {
        private readonly HttpClient _httpClient;

        public AdminProfileServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AdminProfile>> GetAdminsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<AdminPageResponse>("/api/v1/admin/get/all");
            return response?.Content ?? new List<AdminProfile>();
        }

        public async Task<AdminProfile?> GetAdminAsync(long id)
        {
            var response = await _httpClient.GetAsync($"/api/v1/admin/get/{id}");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AdminProfile>();
        }

        public async Task<HttpResponseMessage> CreateAdminAsync(AdminProfile admin)
        {
            return await _httpClient.PostAsJsonAsync("/api/v1/admin/create", admin);
        }

        public async Task<HttpResponseMessage> UpdateAdminAsync(long id, AdminProfile admin)
        {
            return await _httpClient.PatchAsJsonAsync($"/api/v1/admin/update/{id}", admin);
        }

        public async Task<HttpResponseMessage> DeleteAdminAsync(long id)
        {
            return await _httpClient.DeleteAsync($"/api/v1/admin/delete/{id}");
        }
    }
}
