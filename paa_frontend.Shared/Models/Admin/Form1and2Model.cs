using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models.Admin
{
    public class Form1and2Model
    {
        // -------------------------------------------------------------------------
        // Subjects (Form1and2SubjectsDto)
        // -------------------------------------------------------------------------

        public class Form1and2Subject
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("subject")]
            public string? Subject { get; set; } = string.Empty;
        }

        public class PostForm1and2Subject
        {
            [JsonPropertyName("subject")]
            public string? Subject { get; set; } = string.Empty;
        }

        // -------------------------------------------------------------------------
        // Class Names (Form1and2ClassNamesDto)
        // -------------------------------------------------------------------------

        public class Form1and2ClassName
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("className")]
            public string? ClassName { get; set; } = string.Empty;
        }

        public class PostForm1and2ClassName
        {
            [JsonPropertyName("id")]
            public long? Id { get; set; }

            [JsonPropertyName("className")]
            public string? ClassName { get; set; } = string.Empty;
        }

        // -------------------------------------------------------------------------
        // Class Amounts (Form1and2ClassAmountsDto)
        // -------------------------------------------------------------------------

        public class Form1and2ClassAmount
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("form")]
            public int Form { get; set; }

            [JsonPropertyName("className")]
            public string? ClassName { get; set; } = string.Empty;

            [JsonPropertyName("amount")]
            public int Amount { get; set; }
        }
    }
}
