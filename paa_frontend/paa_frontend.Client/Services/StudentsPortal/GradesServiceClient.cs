using System.Net.Http.Json;
using paa_frontend.Shared.Models;

namespace paa_frontend.Client.Services.StudentsPortal
{
    public class GradesServiceClient
    {
        private readonly HttpClient _http;
        private const string Base = "/api/v1/student-grades";

        public GradesServiceClient(HttpClient http)
        {
            _http = http;
        }

        // ── Report Cards ──────────────────────────────────────────

        public async Task<List<ReportCardModel>?> GetReportCardsAsync(long studentId)
        {
            // Fixed: Removed double brackets and fixed the generic nesting
            return await _http.GetFromJsonAsync<List<ReportCardModel>>(
                $"{Base}/report-card/student/get/{studentId}");
        }

        public async Task<ReportCardModel?> GenerateReportCardAsync(ReportCardRequest request, string field = "")
        {
            var response = await _http.PostAsJsonAsync(
                $"{Base}/report-card/generate/add?field={Uri.EscapeDataString(field)}", request);

            response.EnsureSuccessStatusCode();

            // Fixed: Changed << >> to < >
            return await response.Content.ReadFromJsonAsync<ReportCardModel>();
        }

        // ── Marks ─────────────────────────────────────────────────

        public async Task<CourseworkMarksResponse?> GetCourseworkMarksAsync(
            long studentId, int term, int termYear, string field = "default")
        {
            var body = new CourseworkMarksRequest
            {
                StudentId = studentId,
                Term = term,
                TermYear = termYear,
            };
            var response = await _http.PostAsJsonAsync(
                $"{Base}/marks/coursework/student/get?field={Uri.EscapeDataString(field)}", body);

            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CourseworkMarksResponse>();
        }

        public async Task<ExamMarksResponse?> GetExamMarksAsync(
            long studentId, int term, int termYear, string field = "default")
        {
            var body = new ExamMarkRequest // Ensure this matches your model name (ExamMarkRequest vs ExamMarksRequest)
            {
                StudentId = studentId,
                Term = term,
                TermYear = termYear,
            };
            var response = await _http.PostAsJsonAsync(
                $"{Base}/marks/exam/student/get?field={Uri.EscapeDataString(field)}", body);

            if (!response.IsSuccessStatusCode) return null;

            // Fixed: Changed << >> to < >
            return await response.Content.ReadFromJsonAsync<ExamMarksResponse>();
        }

        // ── Assessment ────────────────────────────────────────────

        // ── Marks Entry ───────────────────────────────────────────

        public async Task<bool> SetAssignmentMarkAsync(CourseworkDTO dto)
        {
            var response = await _http.PostAsJsonAsync($"{Base}/marks/coursework/assignment/add", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SetTestMarkAsync(CourseworkDTO dto)
        {
            var response = await _http.PostAsJsonAsync($"{Base}/marks/coursework/test/add", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SetExamMarkAsync(ExamMarkDTO dto)
        {
            var response = await _http.PostAsJsonAsync($"{Base}/marks/exam/add", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EditCourseworkMarkAsync(long id, CourseworkDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"{Base}/marks/coursework/{id}/edit", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EditExamMarkAsync(long examMarkId, ExamMarkDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"{Base}/marks/exam/{examMarkId}/edit", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
