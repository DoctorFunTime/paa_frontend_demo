using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models
{
    public class StudentPassModel
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("nfcCode")]
        public string PassId { get; set; } = string.Empty;

        public string? Condition { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

    public class StudentPassPage
    {
        public List<StudentPassModel> Content { get; set; } = new();
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public int Number { get; set; }
        public int Size { get; set; }
    }
}

namespace paa_frontend.Shared.Models.StudentPassScanner
{
    public class StudentPassModel
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NfcCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public bool IsExpired => ExpirationDate < DateTime.Now;
    }

    public class StudentPassPage
    {
        public List<StudentPassModel> Content { get; set; } = new();
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public int Number { get; set; }
        public int Size { get; set; }
    }

    public class VerificationResult
    {
        public bool Allowed { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string PassId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public Dictionary<string, bool> Checks { get; set; } = new();
        public DateTime VerifiedAt { get; set; }
    }

    public class EntitlementEvent
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class StudentEntitlementModel
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string CheckType { get; set; } = string.Empty;
        public bool Valid { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
