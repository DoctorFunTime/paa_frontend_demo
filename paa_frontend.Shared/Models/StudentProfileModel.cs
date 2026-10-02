using paa_frontend.Shared.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models
{
    public class StudentProfileModel
    {
        public class EmergencyContactDetails
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; } = string.Empty;

            [JsonPropertyName("phone")]
            public string? Phone { get; set; } = string.Empty;

            [JsonPropertyName("email")]
            public string? Email { get; set; } = string.Empty;

            [JsonPropertyName("relationship")]
            public string? Relationship { get; set; } = string.Empty;
        }

        public class Student
        {
            [JsonPropertyName("id")] 
            public int Id { get; set; }    

            [JsonPropertyName("firstName")]
            [Required(ErrorMessage = "First name is required.")]
            public string? FirstName { get; set; } = string.Empty;

            [JsonPropertyName("lastName")]
            [Required(ErrorMessage = "Last name is required.")]
            public string? LastName { get; set; } = string.Empty;

            [JsonPropertyName("email")]
            public string? Email { get; set; } = string.Empty;

            [JsonPropertyName("phone")]
            public string? Phone { get; set; } = string.Empty;

            [JsonPropertyName("address")] 
            public string? Address { get; set; } = string.Empty;

            [JsonConverter(typeof(NullableLocalDateTimeConverter))]
            [JsonPropertyName("dateOfBirth")] 
            public DateTime? DateOfBirth { get; set; }

            [JsonPropertyName("classroom")] 
            [Required(ErrorMessage = "Classroom is required.")]
            public string? Classroom { get; set; } = string.Empty;

            [JsonPropertyName("form")] 
            [Required(ErrorMessage = "Assigned form is required.")]
            public int? Form { get; set; }

            [JsonPropertyName("gender")] 
            public string? Gender { get; set; } = "Other";

            [JsonPropertyName("religion")] 
            public string? Religion { get; set; } = string.Empty;

            [JsonPropertyName("nationalId")] 
            public string? NationalId { get; set; } = string.Empty;

            [JsonPropertyName("tempId")]
            public string? TempId { get; set; } = string.Empty;

            [JsonPropertyName("status")] 
            public string? Status { get; set; } = "Active";

            [JsonConverter(typeof(NullableLocalDateTimeConverter))]
            [JsonPropertyName("createdAt")]
            public DateTime? CreatedAt { get; set; }

            [JsonPropertyName("emergencyContactDetails")]
            public EmergencyContactDetails? EmergencyContactDetails { get; set; }
        }
    }
}
