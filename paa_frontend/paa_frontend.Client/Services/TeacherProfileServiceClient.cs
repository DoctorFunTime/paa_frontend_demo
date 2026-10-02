using paa_frontend.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static paa_frontend.Shared.Models.TeacherProfileModel;

namespace paa_frontend.Client.Services
{
    public class TeacherProfileServiceClient
    {
        private readonly HttpClient _httpClient;

        public TeacherProfileServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Teacher>> GetTeachersAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<TeacherPage>("/api/v1/teacher/get/all");
            return response?.Content ?? new List<Teacher>();
        }

        public async Task<Teacher> GetTeacherAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Teacher>($"/api/v1/teacher/get/{id}");
        }

        public async Task<HttpResponseMessage> CreateTeacherAsync(Teacher teacher)
        {
            return await _httpClient.PostAsJsonAsync("/api/v1/teacher/create", teacher);
        }

        // TeacherProfileServiceClient
        public async Task<AuthCreationResult?> CreateTeacherAndReturnAsync(Teacher teacher)
        {
            var resp = await _httpClient.PostAsJsonAsync("/api/v1/teacher/create", teacher);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<AuthCreationResult>();
        }

        public async Task<HttpResponseMessage> UpdateTeacherAsync(long id, Teacher teacher)
        {
            return await _httpClient.PatchAsJsonAsync($"/api/v1/teacher/update/{id}", teacher);
        }

        public async Task<HttpResponseMessage> DeleteTeacherAsync(long id)
        {
            return await _httpClient.DeleteAsync($"/api/v1/teacher/delete/{id}");
        }

        public async Task<HttpResponseMessage> BulkCreateTeachersAsync(BulkTeacherCreationDto request)
        {
            return await _httpClient.PostAsJsonAsync("/api/v1/teacher/bulk-create", request);
        }
    }
}
