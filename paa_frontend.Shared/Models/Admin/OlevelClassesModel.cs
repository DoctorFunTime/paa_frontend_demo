using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models.Admin
{
    public class OlevelClassesModel
    {
        // -------------------------------------------------------------------------
        // Class Names (Form1to4ClassNamesDto)
        // -------------------------------------------------------------------------

        public class OlevelClassName
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("className")]
            public string? ClassName { get; set; } = string.Empty;
        }

        public class PostOlevelClassName
        {
            [JsonPropertyName("id")]
            public long? Id { get; set; }

            [JsonPropertyName("className")]
            public string? ClassName { get; set; } = string.Empty;
        }

        // -------------------------------------------------------------------------
        // Class Amounts (OlevelClassAmountsDto)
        // -------------------------------------------------------------------------

        public class OlevelClassAmount
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
        // Field Choices (OlevelFieldChoicesDto)
        // -------------------------------------------------------------------------

        public class OlevelFieldChoice
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("studentId")]
            public long StudentId { get; set; }

            [JsonPropertyName("field")]
            public string? Field { get; set; } = string.Empty;
        }

        // -------------------------------------------------------------------------
        // Fields (OlevelFieldsDto)
        // -------------------------------------------------------------------------

        public class OlevelField
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("field")]
            public string? Field { get; set; } = string.Empty;
        }

        public class PostOlevelField
        {
            [JsonPropertyName("id")]
            public long? Id { get; set; }

            [JsonPropertyName("field")]
            public string? Field { get; set; } = string.Empty;
        }
    }

    // -------------------------------------------------------------------------
    // Subjects by Field (OlevelSubjectsDto)
    // Returned by GET /api/v1/enrollment/olevel/field_choices/subjects?field=
    // -------------------------------------------------------------------------

    public class OlevelSubjectsModel
    {
        [JsonPropertyName("field")]
        public string? Field { get; set; } = string.Empty;

        [JsonPropertyName("subjects")]
        public List<string> Subjects { get; set; } = new List<string>();
    }
}
