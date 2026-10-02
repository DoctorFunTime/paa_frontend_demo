using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace paa_frontend.Shared.Models.Admin
{
    public class StudentClassesModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("className")] 
        public string? ClassName { get; set; } = string.Empty;
    }
}
