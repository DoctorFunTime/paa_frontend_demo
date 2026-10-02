using paa_frontend.Shared.Models.Admin;
using System.Net.Http.Json;
using static paa_frontend.Shared.Models.Admin.SubjectsModel;
using static paa_frontend.Shared.Models.StudentProfileModel;

namespace paa_frontend.Client.Services.Admin
{
    public class StudentSubjectServiceClient
    {
        private readonly HttpClient _httpClient;

        public StudentSubjectServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region A levels

        public async Task<bool> PostAlevelSubjectsAsync(List<string> subjects)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/enrollment/alevel/class/subjects/set/all/a_level_subjects", subjects);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            
            return false;
        }

        public async Task<bool> PostAlevelSubjectAsync(string subject)
        {
            // We send the string directly. 
            // "text/plain" is used if the backend doesn't expect JSON quotes.
            using var content = new StringContent(subject, System.Text.Encoding.UTF8, "text/plain");

            var response = await _httpClient.PostAsync("/api/v1/enrollment/alevel/class/subjects/set/a_level_subject", content);

            if (response.IsSuccessStatusCode)
            {
                // Even if the input is text/plain, the output is a Boolean, 
                // so we can still use ReadFromJsonAsync for the result.
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            return false;
        }

        public async Task<List<SubjectsModel.ALevelSubjects>> GetALevelSubjectsAsync()
        {
            // Cache-buster prevents browser from serving stale data after updates
            var url = $"/api/v1/enrollment/alevel/class/subjects/get/all/a_level_subjects?_={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            var response = await _httpClient.GetFromJsonAsync<List<SubjectsModel.ALevelSubjects>>(url);
            return response ?? new List<SubjectsModel.ALevelSubjects>();
        }

        public async Task<List<SubjectsModel.GetStudentALevelSubjectChoices>> GetAllALevelStudentSubjectChoicesAsync()
        {
            var url = $"/api/v1/enrollment/alevel/students/subject_choices/all?_={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            return await _httpClient.GetFromJsonAsync<List<SubjectsModel.GetStudentALevelSubjectChoices>>(url) ?? new();
        }

        public async Task<bool> UpdateALevelSubjectAsync(long subjectId, string subject)
        {
            var content = new StringContent(subject, System.Text.Encoding.UTF8, "text/plain");
            var response = await _httpClient.PatchAsync($"/api/v1/enrollment/alevel/class/subjects/update/a_level_subject/{subjectId}", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }

        public async Task<bool> DeleteALevelSubjectAsync(long subjectId)
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/enrollment/alevel/class/subjects/delete/a_level_subject/{subjectId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }

        //Students subjects api end points
        public async Task<bool> PostALevelSubjectsToStudentAsync(long studentId, List<string> subjects)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/v1/enrollment/alevel/assign/subjects/{studentId}", subjects);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }

        public async Task<List<GetALevelStudent>> GetALevelStudentsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<GetALevelStudent>>("/api/v1/enrollment/alevel/students/all");
            return response ?? new List<GetALevelStudent>();
        }

        public async Task<GetALevelStudent> GetALevelStudentByIdAsync(long studentId)
        {
            var response = await _httpClient.GetFromJsonAsync<GetALevelStudent>($"/api/v1/enrollment/alevel/students/{studentId}");
            return response ?? new GetALevelStudent();
        }

        public async Task<List<GetStudentALevelSubjectChoices>> GetStudentALevelSubjectChoicesAsync()
        {
            // Ensure the student cards do not reuse a cached list after an assignment is saved.
            var url = $"/api/v1/enrollment/alevel/students/subject_choices/all?_={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            var response = await _httpClient.GetFromJsonAsync<List<GetStudentALevelSubjectChoices>>(url);
            return response ?? new List<GetStudentALevelSubjectChoices>();
        }

        public async Task<GetStudentALevelSubjectChoices> GetStudentALevelSubjectChoicesAsync(long Id)
        {
            var url = $"/api/v1/enrollment/alevel/students/subject_choices/{Id}?_={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            using var response = await _httpClient.GetAsync(url);
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound or System.Net.HttpStatusCode.NoContent)
                return new GetStudentALevelSubjectChoices();
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GetStudentALevelSubjectChoices>() ?? new GetStudentALevelSubjectChoices();
        }

        public async Task<bool> UpdateALevelStudentSubjectChoicesAsync(long studentId, GetStudentALevelSubjectChoices studentALevelSubjectChoices)
        {
            // The backend update handler looks up the choice row by student ID.
            var response = await _httpClient.PatchAsJsonAsync($"/api/v1/enrollment/alevel/students/subject_choices/update/{studentId}", studentALevelSubjectChoices);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }

        public async Task<bool> DeleteALevelStudentSubjectChoicesAsync(long Id)
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/enrollment/alevel/students/subject_choices/delete/{Id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }
        #endregion

        #region Forms 1 to 4 class/student subjects

        public async Task<SubjectsModel.Form1to4ClassSubjects?> SetForm1to4ClassSubjectsAsync(SubjectsModel.Form1to4ClassSubjects dto)
        {
            if (dto is null || dto.Form < 1 || dto.Form > 4 || string.IsNullOrWhiteSpace(dto.ClassName))
                return null;

            var response = await _httpClient.PostAsJsonAsync("/api/v1/enrollment/form1to4/class-subjects/set", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<SubjectsModel.Form1to4ClassSubjects>()
                : null;
        }

        public async Task<SubjectsModel.Form1to4ClassSubjects?> GetForm1to4ClassSubjectsAsync(int form, string className)
        {
            if (form < 1 || form > 4 || string.IsNullOrWhiteSpace(className))
                return null;

            var url = $"/api/v1/enrollment/form1to4/class-subjects?form={form}&className={Uri.EscapeDataString(className)}";
            var response = await _httpClient.GetAsync(url);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<SubjectsModel.Form1to4ClassSubjects>()
                : null;
        }

        public async Task<List<SubjectsModel.Form1to4ClassSubjects>> GetAllForm1to4ClassSubjectsAsync()
        {
            var response = await _httpClient.GetAsync("/api/v1/enrollment/form1to4/class-subjects/all");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<SubjectsModel.Form1to4ClassSubjects>>() ?? new()
                : new();
        }

        public async Task<SubjectsModel.Form1to4StudentSubjectChoices?> GetForm1to4StudentSubjectsAsync(long studentId)
        {
            if (studentId <= 0) return null;

            using var response = await _httpClient.GetAsync($"/api/v1/enrollment/form1to4/student-subjects/{studentId}");
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound or System.Net.HttpStatusCode.NoContent)
                return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SubjectsModel.Form1to4StudentSubjectChoices>();
        }

        public async Task<SubjectsModel.Form1to4StudentSubjectChoices?> UpdateForm1to4StudentSubjectsAsync(long studentId, IEnumerable<string> subjects)
        {
            if (studentId <= 0) return null;

            var response = await _httpClient.PutAsJsonAsync(
                $"/api/v1/enrollment/form1to4/student-subjects/{studentId}",
                subjects.Distinct(StringComparer.OrdinalIgnoreCase).ToList());
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<SubjectsModel.Form1to4StudentSubjectChoices>()
                : null;
        }

        #endregion

    }
}
