using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models.Admin
{
    public class AlevelClassesModel
    {
        // -------------------------------------------------------------------------
        // Class Names (Form5to6ClassNamesDto)
        // -------------------------------------------------------------------------

        public class AlevelClassName
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("className")]
            public string? ClassName { get; set; } = string.Empty;
        }

        public class PostAlevelClassName
        {
            [JsonPropertyName("id")]
            public long? Id { get; set; }

            [JsonPropertyName("className")]
            public string? ClassName { get; set; } = string.Empty;
        }

        // -------------------------------------------------------------------------
        // Class Amounts (AlevelClassAmountsDto)
        // -------------------------------------------------------------------------

        public class AlevelClassAmount
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

        // -------------------------------------------------------------------------
        // Student Subject Choices (AlevelStudentSubjectChoicesDto)
        // -------------------------------------------------------------------------

        public class AlevelStudentSubjectChoices
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("studentId")]
            public long StudentId { get; set; }

            [JsonPropertyName("form")]
            public int Form { get; set; }

            [JsonPropertyName("className")]
            public string? ClassName { get; set; } = string.Empty;

            [JsonPropertyName("subjects")]
            public List<string> Subjects { get; set; } = new List<string>();
        }
    }
}
