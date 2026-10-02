using System;
using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models
{
    // ─────────────────────────────────────────────────────────────
    //  STUDENT PASS  (mirrors NormalPassDto.java)
    //  GET /student-pass/{id}
    //  POST /student-pass/create/{id}
    // ─────────────────────────────────────────────────────────────

    public class StudentPassDto
    {
        [JsonPropertyName("id")]             public long?   Id             { get; set; }
        [JsonPropertyName("studentId")]      public long    StudentId      { get; set; }
        [JsonPropertyName("firstName")]      public string? FirstName      { get; set; }
        [JsonPropertyName("lastName")]       public string? LastName       { get; set; }
        [JsonPropertyName("passId")]         public string? PassId         { get; set; }
        [JsonPropertyName("condition")]      public string? Condition      { get; set; }
        [JsonPropertyName("isActive")]       public bool    IsActive       { get; set; }
        [JsonPropertyName("expirationDate")] public string? ExpirationDate { get; set; }
        [JsonPropertyName("createdAt")]      public string? CreatedAt      { get; set; }
        [JsonPropertyName("updatedAt")]      public string? UpdatedAt      { get; set; }

        public string FullName  => $"{FirstName} {LastName}".Trim();
        public bool   IsExpired => ExpirationDate is not null &&
                                   DateTime.TryParse(ExpirationDate, out var exp) &&
                                   exp < DateTime.UtcNow;
    }
}
