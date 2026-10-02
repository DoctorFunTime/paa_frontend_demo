using System.Collections.Generic;
using System.Text.Json.Serialization;
using static paa_frontend.Shared.Models.StudentProfileModel;

namespace paa_frontend.Shared.Models
{
    public class StudentPage
    {
        [JsonPropertyName("content")]       public List<Student> Content       { get; set; } = new();
        [JsonPropertyName("totalPages")]    public int           TotalPages    { get; set; }
        [JsonPropertyName("totalElements")] public int           TotalElements { get; set; }
        [JsonPropertyName("size")]          public int           Size          { get; set; }
        [JsonPropertyName("number")]        public int           Number        { get; set; }
        [JsonPropertyName("first")]         public bool          First         { get; set; }
        [JsonPropertyName("last")]          public bool          Last          { get; set; }
        [JsonPropertyName("empty")]         public bool          Empty         { get; set; }

        // You can add Pageable/Sort if you need them, but not required for basic listing
    }
}
