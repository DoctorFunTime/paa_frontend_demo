using System.Net.Http.Json;
using paa_frontend.Shared.Models;

namespace paa_frontend.Client.Services
{
    public class AttendanceServiceClient
    {
        private readonly HttpClient _http;
        private const string Base = "/api/v1/attendance";

        public AttendanceServiceClient(HttpClient http)
        {
            _http = http;
        }

        // ── Sessions ──────────────────────────────────────────

        public async Task<AttendanceSessionDto?> CreateSessionAsync(CreateAttendanceSessionDto dto)
        {
            var response = await _http.PostAsJsonAsync($"{Base}/session", dto);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                throw new InvalidOperationException("An attendance session already exists for this date and type.");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"CreateSession failed ({(int)response.StatusCode}): {body}");
            }

            var wrapper = await response.Content.ReadFromJsonAsync<AttendanceSessionResponse>();
            return wrapper?.Session;
        }

        public async Task<AttendanceSessionDto?> CreateSessionWithClassAsync(CreateAttendanceSessionDto dto, int form, string classroom)
        {
            var response = await _http.PostAsJsonAsync(
                $"{Base}/session/class?form={form}&classroom={Uri.EscapeDataString(classroom)}", dto);
            if (!response.IsSuccessStatusCode) return null;
            var wrapper = await response.Content.ReadFromJsonAsync<AttendanceSessionResponse>();
            return wrapper?.Session;
        }

        public async Task<AttendanceSessionDto?> GetSessionByIdAsync(long sessionId)
        {
            var wrapper = await _http.GetFromJsonAsync<AttendanceSessionResponse>($"{Base}/session/{sessionId}");
            return wrapper?.Session;
        }

        public async Task<List<AttendanceSessionDto>?> GetSessionsByDateAsync(DateOnly date)
        {
            var wrapper = await _http.GetFromJsonAsync<AttendanceSessionListResponse>($"{Base}/session/date/{date:yyyy-MM-dd}");
            return wrapper?.Sessions;
        }

        public async Task<List<AttendanceSessionDto>?> GetSessionsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            var wrapper = await _http.GetFromJsonAsync<AttendanceSessionListResponse>(
                $"{Base}/session/range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            return wrapper?.Sessions;
        }

        // ── Records ───────────────────────────────────────────

        public async Task<List<AttendanceRecordDto>?> GetRecordsBySessionAsync(long sessionId)
        {
            var wrapper = await _http.GetFromJsonAsync<AttendanceRecordListResponse>($"{Base}/records/session/{sessionId}");
            return wrapper?.Records;
        }

        public async Task<List<AttendanceRecordDto>?> GetRecordsByPersonAsync(long personId)
        {
            var wrapper = await _http.GetFromJsonAsync<AttendanceRecordListResponse>($"{Base}/records/person/{personId}");
            return wrapper?.Records;
        }

        public async Task<List<AttendanceRecordDto>?> GetRecordsByPersonAndDateRangeAsync(long personId, DateOnly startDate, DateOnly endDate)
        {
            var wrapper = await _http.GetFromJsonAsync<AttendanceRecordListResponse>(
                $"{Base}/records/person/{personId}/range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            return wrapper?.Records;
        }

        public async Task<AttendanceRecordDto?> UpdateRecordStatusAsync(long recordId, string status, string? remarks = null)
        {
            var url = $"{Base}/record/{recordId}?status={Uri.EscapeDataString(status)}";
            if (remarks != null) url += $"&remarks={Uri.EscapeDataString(remarks)}";
            var response = await _http.PatchAsync(url, null);
            if (!response.IsSuccessStatusCode) return null;
            var wrapper = await response.Content.ReadFromJsonAsync<AttendanceRecordResponse>();
            return wrapper?.Record;
        }
    }
}
