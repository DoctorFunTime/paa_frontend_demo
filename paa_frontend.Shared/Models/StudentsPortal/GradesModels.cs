using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models
{
    // ─────────────────────────────────────────────────────────────
    //  REPORT CARD  (mirrors ReportCardDTO.java)
    //  GET /report-card/student/{studentId}
    // ─────────────────────────────────────────────────────────────

    public class ReportCardModel
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("studentId")] public long StudentId { get; set; }
        [JsonPropertyName("term")] public string Term { get; set; } = string.Empty;   // e.g. "Term 2"
        [JsonPropertyName("termMonth")] public string? TermMonth { get; set; }                   // e.g. "August"
        [JsonPropertyName("termYear")] public int TermYear { get; set; }
        [JsonPropertyName("generatedDate")] public DateOnly GeneratedDate { get; set; }
        [JsonPropertyName("courseworkWeight")] public double? CourseworkWeight { get; set; } = 0.30;           // 0.0 – 1.0
        [JsonPropertyName("overallComments")] public string? OverallComments { get; set; }
        [JsonPropertyName("studentFirstName")] public string? StudentFirstName { get; set; }
        [JsonPropertyName("studentLastName")] public string? StudentLastName { get; set; }
        [JsonPropertyName("studentForm")] public int? StudentForm { get; set; }
        [JsonPropertyName("studentClass")] public string? StudentClass { get; set; }
        [JsonPropertyName("teacherSubmitted")] public bool TeacherSubmitted { get; set; }
        [JsonPropertyName("adminApproved")] public bool AdminApproved { get; set; }
        [JsonPropertyName("releasedToStudent")] public bool ReleasedToStudent { get; set; }
        [JsonPropertyName("subjects")] public List<ReportCardSubjectModel> Subjects { get; set; } = new();
    }

    // ─────────────────────────────────────────────────────────────
    //  REPORT CARD SUBJECT  (mirrors ReportCardSubjectDTO.java)
    // ─────────────────────────────────────────────────────────────

    public class ReportCardSubjectModel
    {
        [JsonPropertyName("subjectName")] public string? SubjectName { get; set; }
        [JsonPropertyName("examMark")] public double ExamMark { get; set; }
        [JsonPropertyName("courseworkMark")] public double CourseworkMark { get; set; }
        [JsonPropertyName("finalMark")] public double FinalMark { get; set; }
        [JsonPropertyName("grade")] public string? Grade { get; set; }
        [JsonPropertyName("comments")] public string? Comments { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    //  COURSEWORK MARK  (mirrors CourseworkDTO.java)
    //  GET /marks/coursework/student/marks
    // ─────────────────────────────────────────────────────────────

    public class CourseworkDTO
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("studentId")] public long StudentId { get; set; }
        [JsonPropertyName("assignmentId")] public long? AssignmentId { get; set; }
        [JsonPropertyName("testId")] public long? TestId { get; set; }
        [JsonPropertyName("score")] public double? Score { get; set; }
        [JsonPropertyName("feedback")] public string? Feedback { get; set; }
        [JsonPropertyName("gradedDate")] public DateOnly GradedDate { get; set; }
        [JsonPropertyName("teacherId")] public long? TeacherId { get; set; }
        [JsonPropertyName("term")] public string? Term { get; set; }
        [JsonPropertyName("termYear")] public int? TermYear { get; set; }

        // Frontend display helpers (not from backend — derived from context when needed)
        [JsonIgnore] public string? AssignmentTitle { get; set; }
        [JsonIgnore] public string? TestTitle { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    //  EXAM MARK  (mirrors ExamMarkDTO.java)
    //  GET /marks/exam/student/marks
    // ─────────────────────────────────────────────────────────────

    public class ExamMarkDTO
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("studentId")] public long StudentId { get; set; }
        [JsonPropertyName("examId")] public long? ExamId { get; set; }
        [JsonPropertyName("examTitle")] public string? ExamTitle { get; set; }
        /// <summary>Percentage score (0–100). Use for display.</summary>
        [JsonPropertyName("score")] public double? Score { get; set; }
        /// <summary>Raw mark as originally entered. Bind this to your edit input.</summary>
        [JsonPropertyName("actualMark")] public double ActualMark { get; set; }
        /// <summary>Assessment ceiling. Recompute on save: (newRaw / MaxScore) * 100</summary>
        [JsonPropertyName("maxScore")] public double MaxScore { get; set; }
        [JsonPropertyName("feedback")] public string? Feedback { get; set; }
        [JsonPropertyName("gradedDate")] public DateOnly GradedDate { get; set; }
        [JsonPropertyName("teacherId")] public long? TeacherId { get; set; }
        [JsonPropertyName("term")] public string? Term { get; set; }
        [JsonPropertyName("termYear")] public int? TermYear { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    //  REPORT CARD REQUEST  (mirrors ReportCardRequest.java)
    //  POST /report-card/generate?field={field}
    // ─────────────────────────────────────────────────────────────

    public class ReportCardRequest
    {
        [JsonPropertyName("studentId")] public long StudentId { get; set; }
        [JsonPropertyName("term")] public string Term { get; set; } = string.Empty;
        [JsonPropertyName("termMonth")] public string? TermMonth { get; set; }
        [JsonPropertyName("termYear")] public int? TermYear { get; set; }
        [JsonPropertyName("courseworkWeight")] public double CourseworkWeight { get; set; } = 0.30;
        [JsonPropertyName("subjectNames")] public List<string> SubjectNames { get; set; } = new();
        [JsonPropertyName("overallComments")] public string? OverallComments { get; set; }
        [JsonPropertyName("studentFirstName")] public string? StudentFirstName { get; set; }
        [JsonPropertyName("studentLastName")] public string? StudentLastName { get; set; }
        [JsonPropertyName("studentForm")] public int? StudentForm { get; set; }
        [JsonPropertyName("studentClass")] public string? StudentClass { get; set; }
        [JsonPropertyName("customGradingScale")] public Dictionary<string, double>? CustomGradingScale { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    //  SUBJECT COMMENT DTO (used to save bulk teacher comments per student subject)
    // ─────────────────────────────────────────────────────────────
    public class SubjectCommentDTO
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("studentId")] public long StudentId { get; set; }
        [JsonPropertyName("subjectName")] public string SubjectName { get; set; } = string.Empty;
        [JsonPropertyName("className")] public string ClassName { get; set; } = string.Empty;
        [JsonPropertyName("term")] public string Term { get; set; } = string.Empty;
        [JsonPropertyName("termYear")] public int TermYear { get; set; }
        [JsonPropertyName("comments")] public string Comments { get; set; } = string.Empty;
        [JsonPropertyName("teacherId")] public long? TeacherId { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    //  GRADE HELPER  (shared utility used across razor pages)
    // ─────────────────────────────────────────────────────────────

    public static class GradeHelper
    {
        public static string ToGrade(double mark) => mark switch
        {
            >= 80 => "A",
            >= 65 => "B",
            >= 50 => "C",
            >= 40 => "D",
            >= 30 => "E",
            _ => "U"
        };

        public static string ToPillClass(string grade) => grade switch
        {
            "A" => "pill-green",
            "B" => "pill-blue",
            "C" => "pill-blue",
            "D" => "pill-amber",
            "E" => "pill-amber",
            _ => "pill-red"
        };

        public static string ToColor(double mark) => mark switch
        {
            >= 70 => "var(--green)",
            >= 50 => "var(--blue)",
            _ => "var(--red)"
        };

        public static (int Start, int End) TermMonthRange(string term) => term switch
        {
            "Term 1" => (1, 4),
            "Term 2" => (5, 8),
            "Term 3" => (9, 12),
            _ => (1, 12)
        };
    }
}
