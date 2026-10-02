using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models;

public class BulkTeacherCreationDto
{
    [JsonPropertyName("teachers")] public List<TeacherProfileModel.Teacher> Teachers { get; set; } = new();
    [JsonPropertyName("stopOnError")] public bool StopOnError { get; set; }
}

public class BulkTeacherCreationResponseDto
{
    [JsonPropertyName("totalRequested")] public int TotalRequested { get; set; }
    [JsonPropertyName("successCount")] public int SuccessCount { get; set; }
    [JsonPropertyName("failureCount")] public int FailureCount { get; set; }
    [JsonPropertyName("successfulTeachers")] public List<SuccessfulTeacher> SuccessfulTeachers { get; set; } = new();
    [JsonPropertyName("failedTeachers")] public List<FailedTeacher> FailedTeachers { get; set; } = new();
    [JsonPropertyName("processingTimeMs")] public long ProcessingTimeMs { get; set; }

    public class SuccessfulTeacher
    {
        [JsonPropertyName("teacherId")] public long TeacherId { get; set; }
        [JsonPropertyName("firstName")] public string? FirstName { get; set; }
        [JsonPropertyName("lastName")] public string? LastName { get; set; }
        [JsonPropertyName("username")] public string? Username { get; set; }
        [JsonPropertyName("password")] public string? Password { get; set; }
    }

    public class FailedTeacher
    {
        [JsonPropertyName("firstName")] public string? FirstName { get; set; }
        [JsonPropertyName("lastName")] public string? LastName { get; set; }
        [JsonPropertyName("email")] public string? Email { get; set; }
        [JsonPropertyName("reason")] public string? Reason { get; set; }
        [JsonPropertyName("index")] public int Index { get; set; }
    }
}
