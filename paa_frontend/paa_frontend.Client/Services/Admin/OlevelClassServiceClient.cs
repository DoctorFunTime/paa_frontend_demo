using paa_frontend.Shared.Models.Admin;
using System.Net.Http.Json;
using static paa_frontend.Shared.Models.Admin.OlevelClassesModel;

namespace paa_frontend.Client.Services.Admin
{
    public class OlevelClassServiceClient
    {
        private readonly HttpClient _httpClient;

        public OlevelClassServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Class Amounts

        /// <summary>
        /// Gets all O-level class amounts.
        /// GET /api/v1/enrollment/olevel/class/amounts/all
        /// </summary>
        public async Task<List<OlevelClassAmount>> GetAllOlevelClassAmountsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<OlevelClassAmount>>("/api/v1/enrollment/olevel/class/amounts/all");
            return response ?? new List<OlevelClassAmount>();
        }

        /// <summary>
        /// Gets a specific O-level class amount by ID.
        /// GET /api/v1/enrollment/olevel/class/amounts/{id}
        /// </summary>
        public async Task<OlevelClassAmount?> GetOlevelClassAmountByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<OlevelClassAmount>($"/api/v1/enrollment/olevel/class/amounts/{id}");
        }

        #endregion

        #region Class Names

        /// <summary>
        /// Creates a new O-level class name.
        /// POST /api/v1/enrollment/olevel/class/names/create
        /// </summary>
        public async Task<bool> PostOlevelClassNameAsync(PostOlevelClassName dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/enrollment/olevel/class/names/create", dto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }

        /// <summary>
        /// Gets all O-level class names.
        /// GET /api/v1/enrollment/olevel/class/names/all
        /// </summary>
        public async Task<List<OlevelClassName>> GetAllOlevelClassNamesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<OlevelClassName>>("/api/v1/enrollment/olevel/class/names/all");
            return response ?? new List<OlevelClassName>();
        }

        /// <summary>
        /// Gets a specific O-level class name by ID.
        /// GET /api/v1/enrollment/olevel/class/names/{id}
        /// </summary>
        public async Task<OlevelClassName?> GetOlevelClassNameByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<OlevelClassName>($"/api/v1/enrollment/olevel/class/names/{id}");
        }

        /// <summary>
        /// Updates an O-level class name by ID.
        /// PATCH /api/v1/enrollment/olevel/class/names/update/{id}
        /// </summary>
        public async Task<bool> UpdateOlevelClassNameAsync(long id, string name)
        {
            using var content = new StringContent(name, System.Text.Encoding.UTF8, "text/plain");
            var response = await _httpClient.PatchAsync($"/api/v1/enrollment/olevel/class/names/update/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }

        /// <summary>
        /// Deletes an O-level class name by ID.
        /// DELETE /api/v1/enrollment/olevel/class/names/{id}
        /// </summary>
        public async Task<bool> DeleteOlevelClassNameAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/enrollment/olevel/class/names/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }

        #endregion

        #region Field Choices

        /// <summary>
        /// Assigns a field of study to a student.
        /// POST /api/v1/enrollment/olevel/field_choices/{studentId}
        /// </summary>
        public async Task<bool> PostOlevelFieldChoiceAsync(long studentId, string field)
        {
            using var content = new StringContent(field, System.Text.Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync($"/api/v1/enrollment/olevel/field_choices/{studentId}", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }

        /// <summary>
        /// Updates the field of study for a student.
        /// PATCH /api/v1/enrollment/olevel/field_choices/update/{studentId}
        /// </summary>
        public async Task<bool> UpdateOlevelFieldChoiceAsync(long studentId, string field)
        {
            using var content = new StringContent(field, System.Text.Encoding.UTF8, "text/plain");
            var response = await _httpClient.PatchAsync($"/api/v1/enrollment/olevel/field_choices/update/{studentId}", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }

        /// <summary>
        /// Gets the O-level subjects filtered by field.
        /// GET /api/v1/enrollment/olevel/field_choices/subjects?field={field}
        /// </summary>
        public async Task<OlevelSubjectsModel?> GetOlevelSubjectsByFieldAsync(string field)
        {
            return await _httpClient.GetFromJsonAsync<OlevelSubjectsModel>($"/api/v1/enrollment/olevel/field_choices/subjects?field={Uri.EscapeDataString(field)}");
        }

        /// <summary>
        /// Gets the O-Level field of study assigned to a specific student.
        /// GET /api/v1/enrollment/olevel/field_choices/student/{studentId}
        /// Returns null if the student has no field assigned.
        /// </summary>
        public async Task<string?> GetStudentOlevelFieldAsync(long studentId)
        {
            var resp = await _httpClient.GetAsync($"/api/v1/enrollment/olevel/field_choices/student/{studentId}");
            if (!resp.IsSuccessStatusCode) return null;
            var raw = await resp.Content.ReadAsStringAsync();
            return raw.Trim().Trim('"');
        }

        /// <summary>
        /// Creates or updates the field of study for a student depending on whether
        /// a field assignment already exists.
        /// </summary>
        public async Task<(bool Success, bool WasUpdate, string? ExistingField)> SaveStudentOlevelFieldAsync(long studentId, string field)
        {
            var existingField = await GetStudentOlevelFieldAsync(studentId);
            var hasExistingField = !string.IsNullOrWhiteSpace(existingField);

            var success = hasExistingField
                ? await UpdateOlevelFieldChoiceAsync(studentId, field)
                : await PostOlevelFieldChoiceAsync(studentId, field);

            return (success, hasExistingField, existingField);
        }

        /// <summary>
        /// Deletes an O-level field choice by ID.
        /// DELETE /api/v1/enrollment/olevel/field_choices/delete/{id}
        /// </summary>
        public async Task<bool> DeleteOlevelFieldChoiceAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/enrollment/olevel/field_choices/delete/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }

        #endregion

        #region Fields Catalogue

        /// <summary>
        /// Gets all O-level fields (subject areas).
        /// GET /api/v1/enrollment/olevel/fields/get/all
        /// </summary>
        public async Task<List<OlevelField>> GetAllOlevelFieldsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<OlevelField>>("/api/v1/enrollment/olevel/fields/get/all");
            return response ?? new List<OlevelField>();
        }

        /// <summary>
        /// Adds a new O-level field.
        /// POST /api/v1/enrollment/olevel/fields/add
        /// </summary>
        public async Task<bool> PostOlevelFieldAsync(string field)
        {
            var dto = new PostOlevelField { Field = field };
            var response = await _httpClient.PostAsJsonAsync("/api/v1/enrollment/olevel/fields/add", dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<bool>();
            return false;
        }

        /// <summary>
        /// Deletes an O-level field by ID.
        /// DELETE /api/v1/enrollment/olevel/fields/delete/{id}
        /// </summary>
        public async Task<bool> DeleteOlevelFieldAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/enrollment/olevel/fields/delete/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<bool>();
            return false;
        }

        /// <summary>
        /// Updates an O-level field by ID.
        /// PATCH /api/v1/enrollment/olevel/fields/update/{id}
        /// </summary>
        public async Task<bool> UpdateOlevelFieldAsync(long id, string field)
        {
            using var content = new StringContent(field, System.Text.Encoding.UTF8, "text/plain");
            var response = await _httpClient.PatchAsync($"/api/v1/enrollment/olevel/fields/update/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }

        #endregion

    }
}
