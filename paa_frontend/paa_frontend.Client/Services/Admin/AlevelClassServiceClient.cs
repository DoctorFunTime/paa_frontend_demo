using paa_frontend.Shared.Models.Admin;
using System.Net.Http.Json;
using static paa_frontend.Shared.Models.Admin.AlevelClassesModel;

namespace paa_frontend.Client.Services.Admin
{
    public class AlevelClassServiceClient
    {
        private readonly HttpClient _httpClient;

        public AlevelClassServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Class Amounts

        /// <summary>
        /// Gets all A-level class amounts.
        /// GET /api/v1/enrollment/alevel/class/amounts/all
        /// </summary>
        public async Task<List<AlevelClassAmount>> GetAllAlevelClassAmountsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<AlevelClassAmount>>("/api/v1/enrollment/alevel/class/amounts/all");
            return response ?? new List<AlevelClassAmount>();
        }

        /// <summary>
        /// Gets a specific A-level class amount by ID.
        /// GET /api/v1/enrollment/alevel/class/amounts/{id}
        /// </summary>
        public async Task<AlevelClassAmount?> GetAlevelClassAmountByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<AlevelClassAmount>($"/api/v1/enrollment/alevel/class/amounts/{id}");
        }

        #endregion

        #region Class Names

        /// <summary>
        /// Creates a new A-level class name.
        /// POST /api/v1/enrollment/alevel/class/names/create
        /// </summary>
        public async Task<bool> PostAlevelClassNameAsync(PostAlevelClassName dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/enrollment/alevel/class/names/create", dto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }

        /// <summary>
        /// Gets all A-level class names.
        /// GET /api/v1/enrollment/alevel/class/names/all
        /// </summary>
        public async Task<List<AlevelClassName>> GetAllAlevelClassNamesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<AlevelClassName>>("/api/v1/enrollment/alevel/class/names/all");
            return response ?? new List<AlevelClassName>();
        }

        /// <summary>
        /// Gets a specific A-level class name by ID.
        /// GET /api/v1/enrollment/alevel/class/names/{id}
        /// </summary>
        public async Task<AlevelClassName?> GetAlevelClassNameByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<AlevelClassName>($"/api/v1/enrollment/alevel/class/names/{id}");
        }

        /// <summary>
        /// Updates an A-level class name by ID.
        /// PATCH /api/v1/enrollment/alevel/class/names/update/{id}
        /// </summary>
        public async Task<bool> UpdateAlevelClassNameAsync(long id, string name)
        {
            using var content = new StringContent(name, System.Text.Encoding.UTF8, "text/plain");
            var response = await _httpClient.PatchAsync($"/api/v1/enrollment/alevel/class/names/update/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }

        /// <summary>
        /// Deletes an A-level class name by ID.
        /// DELETE /api/v1/enrollment/alevel/class/names/{id}
        /// </summary>
        public async Task<bool> DeleteAlevelClassNameAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/enrollment/alevel/class/names/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }

        #endregion
    }
}
