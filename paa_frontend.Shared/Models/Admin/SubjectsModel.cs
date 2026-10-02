using paa_frontend.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static paa_frontend.Shared.Models.Admin.SubjectsModel;

namespace paa_frontend.Shared.Models.Admin
{
    public class SubjectsModel
    {
        //Form 1 and 2 Subject 
        public class Form1to2Subjects()
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("subject")]
            public string? Subject { get; set; } = string.Empty;
        }

        //Form 3 and 4 Subject
        public class Form3to4Subjects()
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("subject")]
            public string? Subject { get; set; } = string.Empty;

            [JsonPropertyName("field")]
            public string? Field { get; set; } = string.Empty;
        }

        // Forms 1 to 4 class-scoped subject configuration
        public class Form1to4ClassSubjects
        {
            [JsonPropertyName("id")] public long? Id { get; set; }
            [JsonPropertyName("form")] public int Form { get; set; }
            [JsonPropertyName("className")] public string? ClassName { get; set; }
            [JsonPropertyName("classCode")] public string? ClassCode { get; set; }
            [JsonPropertyName("subjects")] public List<string> Subjects { get; set; } = new();
        }

        // Forms 1 to 4 subjects currently enrolled for one student
        public class Form1to4StudentSubjectChoices
        {
            [JsonPropertyName("id")] public long? Id { get; set; }
            [JsonPropertyName("studentId")] public long StudentId { get; set; }
            [JsonPropertyName("form")] public int Form { get; set; }
            [JsonPropertyName("className")] public string? ClassName { get; set; }
            [JsonPropertyName("classCode")] public string? ClassCode { get; set; }
            [JsonPropertyName("subjects")] public List<string> Subjects { get; set; } = new();
        }

        //Form 5 and 6 Subject
        public class ALevelSubjects()
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("subjectName")]
            public string? SubjectName { get; set; } = string.Empty;
        }

        public class GetStudentALevelSubjectChoices
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

        public class PostStudentALevelSubjectsChoices
        {
            [JsonPropertyName("studentId")]
            public long StudentId { get; set; }

            [JsonPropertyName("subjects")]
            public List<string> Subjects { get; set; } = new List<string>();
        }

        public class GetALevelStudent
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("firstName")]
            public string? FirstName { get; set; } = string.Empty;

            [JsonPropertyName("lastName")]
            public string? LastName { get; set; } = string.Empty;

            [JsonPropertyName("form")]
            public int Form { get; set; }

            [JsonPropertyName("classroom")]
            public string? Classroom { get; set; } = string.Empty;

            [JsonPropertyName("status")]
            public string? Status { get; set; } = string.Empty;

            [JsonConverter(typeof(NullableLocalDateTimeConverter))]
            [JsonPropertyName("createdAt")]
            public DateTime? CreatedAt { get; set; } 
        }

    }
    public class StudentALevelSubjectChoicesPage
    {
        [JsonPropertyName("content")]
        public List<PostStudentALevelSubjectsChoices> Content { get; set; } = new List<PostStudentALevelSubjectsChoices>();
        // You can add pagination properties here if needed, e.g.:
        // public int TotalElements { get; set; }
        // public int TotalPages { get; set; }
        // public int PageNumber { get; set; }
        // public int PageSize { get; set; }
    }
}
