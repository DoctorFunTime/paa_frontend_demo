using paa_frontend.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static paa_frontend.Shared.Models.StudentProfileModel;

namespace paa_frontend.Client.Services
{
    public class StudentProfileServiceClient
    {
        private readonly HttpClient _httpClient;

        public StudentProfileServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. Get ALL Students — fetches every page from the Spring paginated endpoint
        public async Task<List<Student>> GetStudentsAsync()
        {
            var all = new List<Student>();
            int page = 0;
            bool isLast = false;

            while (!isLast)
            {
                var response = await _httpClient.GetFromJsonAsync<StudentPage>(
                    $"/api/v1/student/get/all?page={page}&size=2000");

                if (response == null) break;

                if (response.Content?.Count > 0)
                    all.AddRange(response.Content);

                // Prefer Spring's page metadata so this remains correct if the server caps size.
                isLast = response.Last || (response.TotalPages > 0
                    ? page >= response.TotalPages - 1
                    : response.Content == null || response.Content.Count < 2000);
                page++;
            }

            return all;
        }

        /// <summary>Loads only the requested backend page. Use this for directory screens.</summary>
        public async Task<StudentPage> GetStudentsPageAsync(int page, int size = 15)
        {
            var response = await _httpClient.GetFromJsonAsync<StudentPage>(
                $"/api/v1/student/get/all?page={Math.Max(0, page)}&size={Math.Clamp(size, 1, 200)}");
            return response ?? new StudentPage();
        }
        // 2. Get Single Student
        public async Task<Student> GetStudentAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Student>($"/api/v1/student/get/{id}");
        }
        // 3. Create Student
        public async Task<HttpResponseMessage> CreateStudentAsync(Student student)
        {
            return await _httpClient.PostAsJsonAsync("/api/v1/student/create", student);
        }

        public async Task<HttpResponseMessage> BulkCreateStudentsAsync(BulkStudentCreationDto request)
        {
            return await _httpClient.PostAsJsonAsync("/api/v1/student/bulk-create", request);
        }

        // StudentProfileServiceClient
        public async Task<AuthCreationResult?> CreateStudentAndReturnAsync(Student student)
        {
            var resp = await _httpClient.PostAsJsonAsync("/api/v1/student/create", student);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<AuthCreationResult>();
        }

        // 4. Update Student
        public async Task<HttpResponseMessage> UpdateStudentAsync(long id, Student student)
        {
            return await _httpClient.PatchAsJsonAsync($"/api/v1/student/update/{id}", student);
        }

        // 5. Delete Student
        public async Task<HttpResponseMessage> DeleteStudentAsync(long id)
        {
            return await _httpClient.DeleteAsync($"/api/v1/student/delete/{id}");
        }
    }

    // Helper class to match the JSON response shape from your Java backend (Spring Page<Student>)
    public class StudentPage
    {
        public List<Student> Content { get; set; } = new();
        public bool Last { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public int Size { get; set; }
        public int Number { get; set; }
    }
}

