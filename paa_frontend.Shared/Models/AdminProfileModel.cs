using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using paa_frontend.Shared.Helpers;

namespace paa_frontend.Shared.Models
{
    public class AdminProfileModel
    {
        public class Admin
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [Required(ErrorMessage = "First name is required.")]
            [JsonPropertyName("firstName")]
            public string? FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Last name is required.")]
            [JsonPropertyName("lastName")]
            public string? LastName { get; set; } = string.Empty;

            [JsonPropertyName("email")]
            public string? Email { get; set; } = string.Empty;

            [JsonPropertyName("gender")]
            public string? Gender { get; set; } = string.Empty;

            [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone must start with 0 and be exactly 10 digits.")]
            [JsonPropertyName("phone")]
            public string? Phone { get; set; } = string.Empty;

            [JsonPropertyName("nationalId")]
            public string? NationalId { get; set; } = string.Empty;

            [JsonConverter(typeof(NullableLocalDateTimeConverter))]
            [JsonPropertyName("dateOfBirth")]
            public DateTime? DateOfBirth { get; set; }

            [JsonPropertyName("status")]
            public string? Status { get; set; } = string.Empty;
        }

        public class AdminPage
        {
            [JsonPropertyName("content")]
            public List<Admin> Content { get; set; } = new();

            [JsonPropertyName("totalElements")]
            public int TotalElements { get; set; }
        }
    }
}
