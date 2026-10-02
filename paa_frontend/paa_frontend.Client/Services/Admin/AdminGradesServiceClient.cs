using System.Net.Http.Json;
using System.Text.Json;
using paa_frontend.Shared.Models;

namespace paa_frontend.Client.Services.Admin
{
    public class AdminGradesServiceClient
    {
        private readonly HttpClient _http;
        private const string Base = "/api/v1/student-grades";

        public AdminGradesServiceClient(HttpClient http)
        {
            _http = http;
        }

        // ── Report Cards ──────────────────────────────────────────

        public async Task<List<ReportCardModel>?> GetPendingReportCardsAsync()
        {
            var resp = await _http.GetAsync($"{Base}/report-card/pending/get/all");
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<ReportCardModel>>()
                : null;
        }

        public async Task<bool> ApproveReportCardAsync(long reportCardId)
        {
            // Patch requests often need a body; if it's truly null, ensure the API expects that.
            var resp = await _http.PatchAsync($"{Base}/report-card/{reportCardId}/approve/edit", null);
            if (!resp.IsSuccessStatusCode) return false;

            // Note: Ensure the API actually returns a boolean and not just a 200 OK.
            return await resp.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<ReportCardModel>?> GetStudentReportCardsAsync(long studentId)
        {
            var resp = await _http.GetAsync($"{Base}/report-card/admin/student/get/{studentId}");
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<ReportCardModel>>()
                : null;
        }

        public async Task<ReportCardModel?> GenerateReportCardAsync(ReportCardRequest request, string field = "default")
        {
            var resp = await _http.PostAsJsonAsync($"{Base}/report-card/generate/add?field={Uri.EscapeDataString(field)}", request);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(ExtractApiError(body, resp.ReasonPhrase));
            }
            return await resp.Content.ReadFromJsonAsync<ReportCardModel>();
        }

        public async Task<bool> DeleteReportCardAsync(long reportCardId)
        {
            var resp = await _http.DeleteAsync($"{Base}/report-card/delete/{reportCardId}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteReportCardsByStudentIdAsync(long studentId)
        {
            var resp = await _http.DeleteAsync($"{Base}/report-card/student/delete/{studentId}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteReportCardByStudentTermAndYearAsync(long studentId, string term, int termYear)
        {
            var resp = await _http.DeleteAsync($"{Base}/report-card/student/delete/{studentId}/term/{Uri.EscapeDataString(term)}/year/{termYear}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<ReportCardModel?> RegenerateReportCardAsync(ReportCardModel existingCard, string field = "default")
        {
            bool deleted = false;
            if (existingCard.Id.HasValue && existingCard.Id.Value > 0)
            {
                deleted = await DeleteReportCardAsync(existingCard.Id.Value);
            }

            if (!deleted)
            {
                deleted = await DeleteReportCardByStudentTermAndYearAsync(existingCard.StudentId, existingCard.Term, existingCard.TermYear);
            }

            if (!deleted)
                throw new InvalidOperationException("The existing report card could not be removed, so regeneration was stopped to prevent an approved card from remaining active.");

            var req = new ReportCardRequest
            {
                StudentId = existingCard.StudentId,
                Term = existingCard.Term,
                TermMonth = existingCard.TermMonth,
                TermYear = existingCard.TermYear,
                CourseworkWeight = existingCard.CourseworkWeight ?? 0.30,
                OverallComments = existingCard.OverallComments,
                StudentFirstName = existingCard.StudentFirstName,
                StudentLastName = existingCard.StudentLastName,
                StudentForm = existingCard.StudentForm,
                StudentClass = existingCard.StudentClass
            };

            return await GenerateReportCardAsync(req, field);
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

        // ── Marks ─────────────────────────────────────────────────

        public async Task<CourseworkMarksResponse?> GetCourseworkMarksAsync(
            long studentId, int term, int termYear, string field = "")
        {
            var body = new CourseworkMarksRequest
            {
                StudentId = studentId,
                Term = term,
                TermYear = termYear,
            };
            var response = await _http.PostAsJsonAsync(
                $"{Base}/marks/coursework/student/get?field={Uri.EscapeDataString(field)}", body);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<CourseworkMarksResponse>()
                : null;
        }

        public async Task<ExamMarksResponse?> GetExamMarksAsync(
            long studentId, int term, int termYear, string field = "")
        {
            var body = new ExamMarkRequest
            {
                StudentId = studentId,
                Term = term,
                TermYear = termYear,
            };
            var response = await _http.PostAsJsonAsync(
                $"{Base}/marks/exam/student/get?field={Uri.EscapeDataString(field)}", body);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ExamMarksResponse>()
                : null;
        }

        // ── Bulk Generate ─────────────────────────────────────────

        public async Task<(int saved, int errors)> BulkGenerateReportCardsAsync(
            List<long> studentIds, string term, int termYear, double courseworkWeight, string field)
        {
            int saved = 0, errors = 0;
            foreach (var sid in studentIds)
            {
                try
                {
                    var req = new ReportCardRequest
                    {
                        StudentId = sid,
                        Term = term,
                        TermYear = termYear,
                        CourseworkWeight = courseworkWeight
                    };

                    var result = await GenerateReportCardAsync(req, field);
                    if (result != null) saved++;
                    else errors++;
                }
                catch
                {
                    errors++;
                }
            }
            return (saved, errors);
        }
        // ── Grading Scale ─────────────────────────────────────────

        public async Task<List<GradingScaleDTO>?> GetAllGradingScalesAsync()
        {
            var resp = await _http.GetAsync($"{Base}/grading-scale/get/all");
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<GradingScaleDTO>>()
                : null;
        }

        public async Task<List<GradingScaleDTO>?> GetGradingScaleByLevelAsync(string levelType)
        {
            var resp = await _http.GetAsync($"{Base}/grading-scale/get/{Uri.EscapeDataString(levelType)}");
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<GradingScaleDTO>>()
                : null;
        }

        public async Task<List<GradingScaleDTO>?> SaveGradingScaleAsync(string levelType, List<GradingScaleDTO> scales)
        {
            var resp = await _http.PostAsJsonAsync($"{Base}/grading-scale/set/{Uri.EscapeDataString(levelType)}", scales);
            return resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<GradingScaleDTO>>()
                : null;
        }

        // ── All Assessments (for teacher analytics) ───────────────

        public async Task<List<AssignmentsDTO>?> GetAllAssignmentsForLevelAsync(string level, string subject = "", string field = "")
        {
            var resp = await _http.GetAsync($"{Base}/assessment/{Uri.EscapeDataString(level)}/assignment/get/all?subjectName={Uri.EscapeDataString(subject)}&field={Uri.EscapeDataString(field)}");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<AssignmentsDTO>>() : null;
        }

        public async Task<List<TestsDTO>?> GetAllTestsForLevelAsync(string level, string subject = "", string field = "")
        {
            var resp = await _http.GetAsync($"{Base}/assessment/{Uri.EscapeDataString(level)}/test/get/all?subjectName={Uri.EscapeDataString(subject)}&field={Uri.EscapeDataString(field)}");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<TestsDTO>>() : null;
        }

        public async Task<List<ExamsDTO>?> GetAllExamsForLevelAsync(string level, string subject = "", string field = "")
        {
            var resp = await _http.GetAsync($"{Base}/assessment/{Uri.EscapeDataString(level)}/exam/get/all?subjectName={Uri.EscapeDataString(subject)}&field={Uri.EscapeDataString(field)}");
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<List<ExamsDTO>>() : null;
        }
    }
}
