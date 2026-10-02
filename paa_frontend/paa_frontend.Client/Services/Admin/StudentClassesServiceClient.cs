using System.Net.Http.Json;
using static paa_frontend.Shared.Models.Admin.OlevelClassesModel;

namespace paa_frontend.Client.Services.Admin
{
    public class StudentClassesServiceClient
    {
        private readonly HttpClient _httpClient;

        public StudentClassesServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Class Amounts

        /// <summary>
        /// Gets all unified Forms 1-4 class amounts.
        /// GET /api/v1/enrollment/olevel/class/amounts/all
        /// </summary>
        public async Task<List<OlevelClassAmount>> GetAllForm1to4ClassAmountsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<OlevelClassAmount>>("/api/v1/enrollment/olevel/class/amounts/all");
            return response ?? new List<OlevelClassAmount>();
        }

        #endregion

        #region Class Names

        /// <summary>
        /// Gets all unified Forms 1-4 class names.
        /// GET /api/v1/enrollment/olevel/class/names/all
        /// </summary>
        public async Task<List<OlevelClassName>> GetAllForm1to4ClassNamesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<OlevelClassName>>("/api/v1/enrollment/olevel/class/names/all");
            return response ?? new List<OlevelClassName>();
        }

        /// <summary>
        /// Creates a new unified Forms 1-4 class name.
        /// POST /api/v1/enrollment/olevel/class/names/create
        /// </summary>
        public async Task<bool> PostForm1to4ClassNameAsync(PostOlevelClassName dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/enrollment/olevel/class/names/create", dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<bool>();
            return false;
        }

        /// <summary>
        /// Updates a unified Forms 1-4 class name by ID.
        /// PATCH /api/v1/enrollment/olevel/class/names/update/{id}
        /// </summary>
        public async Task<bool> UpdateForm1to4ClassNameAsync(long id, string name)
        {
            using var content = new StringContent(name, System.Text.Encoding.UTF8, "text/plain");
            var response = await _httpClient.PatchAsync($"/api/v1/enrollment/olevel/class/names/update/{id}", content);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<bool>();
            return false;
        }

        /// <summary>
        /// Deletes a unified Forms 1-4 class name by ID.
        /// DELETE /api/v1/enrollment/olevel/class/names/{id}
        /// </summary>
        public async Task<bool> DeleteForm1to4ClassNameAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/enrollment/olevel/class/names/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<bool>();
            return false;
        }

        #endregion
    }
}
