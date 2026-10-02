using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using paa_frontend.Shared.Helpers;

namespace paa_frontend.Shared.Models
{
    public class BurserProfileModel
    {
        public class Burser
        {
            [JsonPropertyName("id")] 
            public long Id { get; set; }

            [JsonPropertyName("firstName")]
            [Required(ErrorMessage = "First name is required.")]
            public string? FirstName { get; set; } = string.Empty;

            [JsonPropertyName("lastName")]
            [Required(ErrorMessage = "Last name is required.")]
            public string? LastName { get; set; } = string.Empty;

            [JsonPropertyName("email")]
            public string? Email { get; set; } = string.Empty;

            [JsonPropertyName("gender")] 
            [Required(ErrorMessage = "Gender is required.")]
            public string? Gender { get; set; } = string.Empty;

            [JsonPropertyName("phone")] 
            [Required(ErrorMessage = "Phone is required.")]
            [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone must start with 0 and be exactly 10 digits.")] 
            public string? Phone { get; set; } = string.Empty;

            [JsonPropertyName("nationalId")] 
            [Required(ErrorMessage = "National ID is required.")]
            public string? NationalId { get; set; } = string.Empty;


            [JsonConverter(typeof(NullableLocalDateTimeConverter))]
            [JsonPropertyName("dateOfBirth")] 
            [Required(ErrorMessage = "Date of birth is required.")]
            public DateTime? DateOfBirth { get; set; }
            
            [JsonPropertyName("status")] 
            [Required(ErrorMessage = "Employment status is required.")]
            public string? Status { get; set; } = string.Empty;
        }

        public class BurserPage
        {
            [JsonPropertyName("content")] 
            public System.Collections.Generic.List<Burser> Content { get; set; } = new();
            [JsonPropertyName("totalElements")] 
            public int TotalElements { get; set; }
        }
    }
}
