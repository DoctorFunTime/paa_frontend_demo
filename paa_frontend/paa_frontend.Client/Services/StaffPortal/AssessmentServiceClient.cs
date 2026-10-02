using System.Net.Http.Json;
using System.Text.Json;
using paa_frontend.Shared.Models;

namespace paa_frontend.Client.Services.StaffPortal
{
    public class AssessmentServiceClient
    {
        private readonly HttpClient _http;
        private const string Base = "/api/v1/student-grades";

        public AssessmentServiceClient(HttpClient http)
        {
            _http = http;
        }

        // ── Assignments ──────────────────────────────────────────

        public Task<bool> AddAlevelAssignmentAsync(string subjectName, AssignmentsDTO dto)
        {
            if (string.IsNullOrWhiteSpace(subjectName) || string.IsNullOrWhiteSpace(dto?.Title))
                return Task.FromResult(false);
            return PostBoolAsync($"{Base}/assessment/alevel/assignment/add?subjectName={Uri.EscapeDataString(subjectName)}", dto);
        }

        public Task<bool> AddForm1to4AssignmentAsync(string subjectName, AssignmentsDTO dto)
        {
            if (string.IsNullOrWhiteSpace(subjectName) || string.IsNullOrWhiteSpace(dto?.Title))
                return Task.FromResult(false);
            return PostBoolAsync($"{Base}/assessment/form1to4/assignment/add?subjectName={Uri.EscapeDataString(subjectName)}", dto);
        }

        // ── Tests ────────────────────────────────────────────────

        public Task<bool> AddAlevelTestAsync(string subjectName, TestsDTO dto)
        {
            if (string.IsNullOrWhiteSpace(subjectName) || string.IsNullOrWhiteSpace(dto?.Title))
                return Task.FromResult(false);
            return PostBoolAsync($"{Base}/assessment/alevel/test/add?subjectName={Uri.EscapeDataString(subjectName)}", dto);
        }

        public Task<bool> AddForm1to4TestAsync(string subjectName, TestsDTO dto)
        {
            if (string.IsNullOrWhiteSpace(subjectName) || string.IsNullOrWhiteSpace(dto?.Title))
                return Task.FromResult(false);
            return PostBoolAsync($"{Base}/assessment/form1to4/test/add?subjectName={Uri.EscapeDataString(subjectName)}", dto);
        }

        // ── Exams ────────────────────────────────────────────────

        public Task<bool> AddAlevelExamAsync(string subjectName, ExamsDTO dto)
        {
            if (string.IsNullOrWhiteSpace(subjectName) || string.IsNullOrWhiteSpace(dto?.Title))
                return Task.FromResult(false);
            return PostBoolAsync($"{Base}/assessment/alevel/exam/add?subjectName={Uri.EscapeDataString(subjectName)}", dto);
        }

        public Task<bool> AddForm1to4ExamAsync(string subjectName, ExamsDTO dto)
        {
            if (string.IsNullOrWhiteSpace(subjectName) || string.IsNullOrWhiteSpace(dto?.Title))
                return Task.FromResult(false);
            return PostBoolAsync($"{Base}/assessment/form1to4/exam/add?subjectName={Uri.EscapeDataString(subjectName)}", dto);
        }

        // ── Assessment Queries ───────────────────────────────────

        public Task<List<AssignmentsDTO>?> GetAssignmentsAsync(string level, string subjectName = "", string field = "")
            => GetAssessmentListAsync<AssignmentsDTO>(
                $"{Base}/assessment/{Uri.EscapeDataString(level)}/assignment/get/all?subjectName={Uri.EscapeDataString(subjectName ?? "")}&field={Uri.EscapeDataString(field ?? "")}");

        public Task<List<TestsDTO>?> GetTestsAsync(string level, string subjectName = "", string field = "")
            => GetAssessmentListAsync<TestsDTO>(
                $"{Base}/assessment/{Uri.EscapeDataString(level)}/test/get/all?subjectName={Uri.EscapeDataString(subjectName ?? "")}&field={Uri.EscapeDataString(field ?? "")}");

        public Task<List<ExamsDTO>?> GetExamsAsync(string level, string subjectName = "", string field = "")
            => GetAssessmentListAsync<ExamsDTO>(
                $"{Base}/assessment/{Uri.EscapeDataString(level)}/exam/get/all?subjectName={Uri.EscapeDataString(subjectName ?? "")}&field={Uri.EscapeDataString(field ?? "")}");

        private async Task<List<T>?> GetAssessmentListAsync<T>(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            var response = await _http.GetAsync(url);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<T>>()
                : null;
        }

        // ── Edit Assessment Info ─────────────────────────────────

        public Task<bool> EditAssignmentAsync(long id, AssignmentsDTO dto)
            => PutBoolAsync($"{Base}/assessment/assignment/{id}/edit", dto);

        public Task<bool> EditTestAsync(long id, TestsDTO dto)
            => PutBoolAsync($"{Base}/assessment/test/{id}/edit", dto);

        public Task<bool> EditExamAsync(long id, ExamsDTO dto)
            => PutBoolAsync($"{Base}/assessment/exam/{id}/edit", dto);

        // ── Delete Assessment (Admin) ────────────────────────────

        public async Task<bool> DeleteAssignmentAsync(long id)
        {
            var resp = await _http.DeleteAsync($"{Base}/assessment/assignment/{id}/delete");
            if (!resp.IsSuccessStatusCode)
                resp = await _http.DeleteAsync($"{Base}/assessment/assignment/{id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTestAsync(long id)
        {
            var resp = await _http.DeleteAsync($"{Base}/assessment/test/{id}/delete");
            if (!resp.IsSuccessStatusCode)
                resp = await _http.DeleteAsync($"{Base}/assessment/test/{id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteExamAsync(long id)
        {
            var resp = await _http.DeleteAsync($"{Base}/assessment/exam/{id}/delete");
            if (!resp.IsSuccessStatusCode)
                resp = await _http.DeleteAsync($"{Base}/assessment/exam/{id}");
            return resp.IsSuccessStatusCode;
        }

        public Task<bool> DeleteAssessmentAsync(string type, long id) => type?.ToLowerInvariant() switch
        {
            "assignment" => DeleteAssignmentAsync(id),
            "test"       => DeleteTestAsync(id),
            "exam"       => DeleteExamAsync(id),
            _            => Task.FromResult(false)
        };

        public async Task<bool> DeleteCourseworkMarkAsync(long courseworkId)
        {
            var resp = await _http.DeleteAsync($"{Base}/marks/coursework/delete/{courseworkId}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteExamMarkAsync(long examMarkId)
        {
            var resp = await _http.DeleteAsync($"{Base}/marks/exam/delete/{examMarkId}");
            return resp.IsSuccessStatusCode;
        }

        // ── Marks ────────────────────────────────────────────────

        public async Task<CourseworkDTO?> SetAssignmentMarkAsync(CourseworkDTO dto)
        {
            if (dto?.StudentId == 0 || dto?.AssignmentId == null || dto?.Score < 0) return null;
            var resp = await _http.PostAsJsonAsync($"{Base}/marks/coursework/assignment/add", dto);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<CourseworkDTO>()
                : null;
        }

        public async Task<CourseworkDTO?> SetTestMarkAsync(CourseworkDTO dto)
        {
            if (dto?.StudentId == 0 || dto?.TestId == null || dto?.Score < 0) return null;
            var resp = await _http.PostAsJsonAsync($"{Base}/marks/coursework/test/add", dto);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<CourseworkDTO>()
                : null;
        }

        public async Task<ExamMarkDTO?> SetExamMarkAsync(ExamMarkDTO dto)
        {
            if (dto?.StudentId == 0 || dto?.ExamId == 0 || dto?.Score < 0) return null;
            var resp = await _http.PostAsJsonAsync($"{Base}/marks/exam/add", dto);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<ExamMarkDTO>()
                : null;
        }

        public async Task<CourseworkDTO?> EditCourseworkMarkAsync(long courseworkId, CourseworkDTO dto)
        {
            var resp = await _http.PutAsJsonAsync($"{Base}/marks/coursework/{courseworkId}/edit", dto);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<CourseworkDTO>()
                : null;
        }

        public async Task<ExamMarkDTO?> EditExamMarkAsync(long examMarkId, ExamMarkDTO dto)
        {
            var resp = await _http.PutAsJsonAsync($"{Base}/marks/exam/{examMarkId}/edit", dto);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<ExamMarkDTO>()
                : null;
        }

        public async Task<CourseworkDTO?> EditCourseworkFeedbackAsync(long courseworkId, CourseworkDTO dto)
        {
            var resp = await _http.PutAsJsonAsync($"{Base}/marks/coursework/{courseworkId}/feedback/edit", dto);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<CourseworkDTO>()
                : null;
        }

        public async Task<ExamMarkDTO?> EditExamFeedbackAsync(long examMarkId, ExamMarkDTO dto)
        {
            var resp = await _http.PutAsJsonAsync($"{Base}/marks/exam/{examMarkId}/feedback/edit", dto);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<ExamMarkDTO>()
                : null;
        }
        // ── Bulk Marks ───────────────────────────────────────────

        public async Task<List<CourseworkDTO>?> BulkSetCourseworkMarksAsync(List<CourseworkDTO> marks)
        {
            var resp = await _http.PostAsJsonAsync($"{Base}/marks/coursework/bulk/add", marks);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<CourseworkDTO>>()
                : null;
        }

        public async Task<List<ExamMarkDTO>?> BulkSetExamMarksAsync(List<ExamMarkDTO> marks)
        {
            var resp = await _http.PostAsJsonAsync($"{Base}/marks/exam/bulk/add", marks);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<ExamMarkDTO>>()
                : null;
        }

        public async Task<List<CourseworkDTO>?> BulkEditCourseworkMarksAsync(List<CourseworkDTO> marks)
        {
            var resp = await _http.PutAsJsonAsync($"{Base}/marks/coursework/bulk/edit", marks);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<CourseworkDTO>>()
                : null;
        }

        public async Task<List<ExamMarkDTO>?> BulkEditExamMarksAsync(List<ExamMarkDTO> marks)
        {
            var resp = await _http.PutAsJsonAsync($"{Base}/marks/exam/bulk/edit", marks);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<ExamMarkDTO>>()
                : null;
        }

        public async Task<CourseworkMarksResponse?> GetCourseworkMarksAsync(CourseworkMarksRequest request, string field)
        {
            var resp = await _http.PostAsJsonAsync(
                $"{Base}/marks/coursework/student/get?field={Uri.EscapeDataString(field)}", request);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<CourseworkMarksResponse>()
                : null;
        }

        public async Task<ExamMarksResponse?> GetExamMarksAsync(ExamMarkRequest request, string field)
        {
            var resp = await _http.PostAsJsonAsync(
                $"{Base}/marks/exam/student/get?field={Uri.EscapeDataString(field)}", request);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<ExamMarksResponse>()
                : null;
        }

        // ── Single Assessment Marks ──────────────────────────────

        public async Task<List<CourseworkDTO>?> GetMarksByAssignmentAsync(long assignmentId)
        {
            var resp = await _http.GetAsync($"{Base}/marks/coursework/assignment/{assignmentId}");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<CourseworkDTO>>() : null;
        }

        public async Task<List<CourseworkDTO>?> GetMarksByTestAsync(long testId)
        {
            var resp = await _http.GetAsync($"{Base}/marks/coursework/test/{testId}");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<CourseworkDTO>>() : null;
        }

        public async Task<List<ExamMarkDTO>?> GetMarksByExamAsync(long examId)
        {
            var resp = await _http.GetAsync($"{Base}/marks/exam/{examId}");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<ExamMarkDTO>>() : null;
        }

        // ── Report Cards ─────────────────────────────────────────

        public string? LastReportCardError { get; private set; }

        public async Task<ReportCardModel?> GenerateReportCardAsync(ReportCardRequest request, string field)
        {
            LastReportCardError = null;
            var response = await _http.PostAsJsonAsync(
                $"{Base}/report-card/generate/add?field={Uri.EscapeDataString(field)}", request);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                LastReportCardError = ExtractApiError(body, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ReportCardModel>();
        }

        private static string ExtractApiError(string body, string? fallback)
        {
            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    using var document = JsonDocument.Parse(body);
                    if (document.RootElement.TryGetProperty("error", out var error) &&
                        !string.IsNullOrWhiteSpace(error.GetString()))
                    {
                        return error.GetString()!;
                    }
                }
                catch (JsonException) { }
            }

            return string.IsNullOrWhiteSpace(fallback)
                ? "Report card generation failed."
                : $"Report card generation failed: {fallback}.";
        }

        public async Task<List<ReportCardModel>?> GetStudentReportCardsAsync(long studentId)
        {
            var resp = await _http.GetAsync($"{Base}/report-card/student/get/{studentId}");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<ReportCardModel>>() : null;
        }

        public async Task<List<ReportCardModel>?> GetPendingReportCardsAsync()
        {
            var resp = await _http.GetAsync($"{Base}/report-card/pending/get/all");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<ReportCardModel>>() : null;
        }

        public async Task<bool> ApproveReportCardAsync(long reportCardId)
        {
            var resp = await _http.PatchAsync($"{Base}/report-card/{reportCardId}/approve/edit", null);
            if (!resp.IsSuccessStatusCode) return false;
            return await resp.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<ReportCardModel>?> GetAdminStudentReportCardsAsync(long studentId)
        {
            var resp = await _http.GetAsync($"{Base}/report-card/admin/student/get/{studentId}");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<ReportCardModel>>() : null;
        }

        // ── Subject Comments ─────────────────────────────────────

        public async Task<List<SubjectCommentDTO>?> GetSubjectCommentsAsync(string subjectName, string className, string term, int year)
        {
            var url = $"{Base}/comments/subject/get?subjectName={Uri.EscapeDataString(subjectName)}&className={Uri.EscapeDataString(className)}&term={Uri.EscapeDataString(term)}&year={year}";
            var resp = await _http.GetAsync(url);
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<SubjectCommentDTO>>() : null;
        }

        public async Task<bool> SaveSubjectCommentsAsync(List<SubjectCommentDTO> comments)
        {
            var resp = await _http.PostAsJsonAsync($"{Base}/comments/subject/save", comments);
            return resp.IsSuccessStatusCode;
        }

        // ── Helpers ──────────────────────────────────────────────

        private async Task<bool> PostBoolAsync<T>(string url, T body)
        {
            var resp = await _http.PostAsJsonAsync(url, body);
            if (!resp.IsSuccessStatusCode) return false;
            return await resp.Content.ReadFromJsonAsync<bool>();
        }

        private async Task<bool> PutBoolAsync<T>(string url, T body)
        {
            var resp = await _http.PutAsJsonAsync(url, body);
            if (!resp.IsSuccessStatusCode) return false;
            return await resp.Content.ReadFromJsonAsync<bool>();
        }
    }
}
