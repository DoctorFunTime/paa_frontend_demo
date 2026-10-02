using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models
{
    public class BulkStudentCreationDto
    {
        [JsonPropertyName("students")]
        public List<StudentProfileModel.Student> Students { get; set; } = new();

        [JsonPropertyName("stopOnError")]
        public bool StopOnError { get; set; }
    }

    public class BulkCreationResponseDto
    {
        [JsonPropertyName("totalRequested")]
        public int TotalRequested { get; set; }

        [JsonPropertyName("successCount")]
        public int SuccessCount { get; set; }

        [JsonPropertyName("failureCount")]
        public int FailureCount { get; set; }

        [JsonPropertyName("successfulStudents")]
        public List<SuccessfulStudent> SuccessfulStudents { get; set; } = new();

        [JsonPropertyName("failedStudents")]
        public List<FailedStudent> FailedStudents { get; set; } = new();

        [JsonPropertyName("processingTimeMs")]
        public long ProcessingTimeMs { get; set; }

        public class SuccessfulStudent
        {
            [JsonPropertyName("studentId")]
            public long StudentId { get; set; }

            [JsonPropertyName("firstName")]
            public string? FirstName { get; set; }

            [JsonPropertyName("lastName")]
            public string? LastName { get; set; }

            [JsonPropertyName("username")]
            public string? Username { get; set; }

            [JsonPropertyName("password")]
            public string? Password { get; set; }
        }

        public class FailedStudent
        {
            [JsonPropertyName("firstName")]
            public string? FirstName { get; set; }

            [JsonPropertyName("lastName")]
            public string? LastName { get; set; }

            [JsonPropertyName("email")]
            public string? Email { get; set; }

            [JsonPropertyName("reason")]
            public string? Reason { get; set; }

            [JsonPropertyName("index")]
            public int Index { get; set; }
        }
    }
}
