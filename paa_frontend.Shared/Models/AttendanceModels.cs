using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models
{
    public class AttendanceSessionDto
    {
        [JsonPropertyName("id")] public long Id { get; set; }
        [JsonPropertyName("date")] public DateOnly Date { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; } = "Student";
        [JsonPropertyName("referenceId")] public long ReferenceId { get; set; }
        [JsonPropertyName("createdBy")] public long CreatedBy { get; set; }
        [JsonPropertyName("records")] public List<AttendanceRecordDto>? Records { get; set; }
    }

    public class AttendanceRecordDto
    {
        [JsonPropertyName("id")] public long Id { get; set; }
        [JsonPropertyName("sessionId")] public long SessionId { get; set; }
        [JsonPropertyName("personId")] public long PersonId { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; } = "Present";
        [JsonPropertyName("remarks")] public string? Remarks { get; set; }
    }

    public class CreateAttendanceSessionDto
    {
        [JsonPropertyName("date")] public DateOnly Date { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; } = "Student";
        [JsonPropertyName("referenceId")] public long ReferenceId { get; set; }
        [JsonPropertyName("createdBy")] public long CreatedBy { get; set; }
        [JsonPropertyName("records")] public List<AttendanceRecordInputDto>? Records { get; set; }
    }

    public class AttendanceRecordInputDto
    {
        [JsonPropertyName("personId")] public long PersonId { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
        [JsonPropertyName("personType")] public string? PersonType { get; set; }
        [JsonPropertyName("remarks")] public string? Remarks { get; set; }
    }

    public class AttendanceSessionListResponse
    {
        [JsonPropertyName("sessions")] public List<AttendanceSessionDto>? Sessions { get; set; }
    }

    public class AttendanceSessionResponse
    {
        [JsonPropertyName("session")] public AttendanceSessionDto? Session { get; set; }
    }

    public class AttendanceRecordListResponse
    {
        [JsonPropertyName("records")] public List<AttendanceRecordDto>? Records { get; set; }
    }

    public class AttendanceRecordResponse
    {
        [JsonPropertyName("record")] public AttendanceRecordDto? Record { get; set; }
    }
}
