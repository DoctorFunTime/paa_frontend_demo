using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models
{
    // ─────────────────────────────────────────────────────────────
    //  ASSIGNMENTS DTO  (mirrors AssignmentsDTO.java)
    //  Used by POST /assessment/olevel/assignment
    //           POST /assessment/alevel/assignment
    //           POST /assessment/form1and2/assignment
    // ─────────────────────────────────────────────────────────────

    public class AssignmentsDTO
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        // Forms 1–4 assessments are keyed by subjectName. The legacy subject
        // catalogue ID may legitimately be null in the new setup.
        [JsonPropertyName("subjectId")] public long? SubjectId { get; set; }
        [JsonPropertyName("subjectName")] public string? SubjectName { get; set; }
        [JsonPropertyName("title")] public string? Title { get; set; }
        [JsonPropertyName("maxScore")] public double MaxScore { get; set; } = 100;
        [JsonPropertyName("weight")] public double Weight { get; set; } = 0.20;
        // Existing assessments created before date persistence was added may
        // legitimately have no date. Keep the response model nullable so one
        // old row cannot prevent the whole assessment list from loading.
        [JsonPropertyName("dueDate")] public DateOnly? DueDate { get; set; }
        [JsonPropertyName("teacherId")] public long? TeacherId { get; set; }
        [JsonPropertyName("level")] public string? Level { get; set; }  // "form1and2", "olevel", "alevel"
        [JsonPropertyName("form")] public int? Form { get; set; }
        [JsonPropertyName("className")] public string? ClassName { get; set; }
        [JsonPropertyName("term")] public string? Term { get; set; }
        [JsonPropertyName("termYear")] public int? TermYear { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    //  TESTS DTO  (mirrors TestsDTO.java)
    //  Used by POST /assessment/olevel/test
    //           POST /assessment/alevel/test
    //           POST /assessment/form1and2/test
    // ─────────────────────────────────────────────────────────────

    public class TestsDTO
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("subjectId")] public long? SubjectId { get; set; }
        [JsonPropertyName("subjectName")] public string? SubjectName { get; set; }
        [JsonPropertyName("title")] public string? Title { get; set; }
        [JsonPropertyName("maxScore")] public double MaxScore { get; set; } = 100;
        [JsonPropertyName("weight")] public double Weight { get; set; } = 0.20;
        [JsonPropertyName("testDate")] public DateOnly? TestDate { get; set; }
        [JsonPropertyName("teacherId")] public long? TeacherId { get; set; }
        [JsonPropertyName("level")] public string? Level { get; set; }  // "form1and2", "olevel", "alevel"
        [JsonPropertyName("form")] public int? Form { get; set; }
        [JsonPropertyName("className")] public string? ClassName { get; set; }
        [JsonPropertyName("term")] public string? Term { get; set; }
        [JsonPropertyName("termYear")] public int? TermYear { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    //  EXAMS DTO  (mirrors ExamsDTO.java)
    //  Used by POST /assessment/olevel/exam
    //           POST /assessment/alevel/exam
    //           POST /assessment/form1and2/exam
    // ─────────────────────────────────────────────────────────────

    public class ExamsDTO
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("subjectId")] public long? SubjectId { get; set; }
        [JsonPropertyName("subjectName")] public string? SubjectName { get; set; }
        [JsonPropertyName("title")] public string? Title { get; set; }
        [JsonPropertyName("maxScore")] public double MaxScore { get; set; } = 100;
        [JsonPropertyName("weight")] public double Weight { get; set; } = 0.20;
        [JsonPropertyName("examDate")] public DateOnly? ExamDate { get; set; }
        [JsonPropertyName("teacherId")] public long? TeacherId { get; set; }
        [JsonPropertyName("level")] public string? Level { get; set; }  // "form1and2", "olevel", "alevel"
        [JsonPropertyName("form")] public int? Form { get; set; }
        [JsonPropertyName("className")] public string? ClassName { get; set; }
        [JsonPropertyName("term")] public string? Term { get; set; }
        [JsonPropertyName("termYear")] public int? TermYear { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    //  COURSEWORK MARKS REQUEST  (mirrors CourseworkMarksRequest.java)
    //  Sent to GET /marks/coursework/student/marks?field={field}
    //  Note: Java uses Integer term (1, 2, 3) — not "Term 1" string
    // ─────────────────────────────────────────────────────────────

    public class CourseworkMarksRequest
    {
        [JsonPropertyName("studentId")] public long StudentId { get; set; }
        [JsonPropertyName("term")] public int Term { get; set; }   // 1, 2, or 3
        [JsonPropertyName("termMonth")] public string? TermMonth { get; set; }
        [JsonPropertyName("termYear")] public int? TermYear { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    //  EXAM MARK REQUEST  (mirrors ExamMarkRequest.java)
    //  Sent to GET /marks/exam/student/marks?field={field}
    // ─────────────────────────────────────────────────────────────

    public class ExamMarkRequest
    {
        [JsonPropertyName("studentId")] public long StudentId { get; set; }
        [JsonPropertyName("term")] public int Term { get; set; }   // 1, 2, or 3
        [JsonPropertyName("termMonth")] public string? TermMonth { get; set; }
        [JsonPropertyName("termYear")] public int? TermYear { get; set; }
    }


    // ─────────────────────────────────────────────────────────────
    //  COURSEWORK WEIGHT DTO  (mirrors CourseWorkWeightDTO.java)
    // ─────────────────────────────────────────────────────────────

    public class CourseworkWeightDto
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("courseworkWeight")] public double? CourseworkWeight { get; set; }  // e.g. 0.30
    }

    // ─────────────────────────────────────────────────────────────
    //  EXAM WEIGHT DTO  (mirrors ExamWeightDTO.java)
    // ─────────────────────────────────────────────────────────────

    public class ExamWeightDto
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("examWeight")] public double? ExamWeight { get; set; }  // e.g. 0.70
    }

    // ─────────────────────────────────────────────────────────────
    //  MARK ENTRY  — one scored entry returned by the backend.
    //  The backend stores marks as a percentage internally but
    //  also sends back the raw actualMark and the maxScore so the
    //  frontend can reconstruct / edit the original raw value:
    //
    //    rawMark  = actualMark          (use this for the edit input)
    //    maxScore = maxScore            (the assessment ceiling)
    //    display% = percentageScore     (what you show in read-only views)
    //
    //  Recalculate on save: percentageScore = (newRaw / maxScore) * 100
    // ─────────────────────────────────────────────────────────────

    public class MarkEntry
    {
        [JsonPropertyName("percentageScore")] public double PercentageScore { get; set; }
        [JsonPropertyName("actualMark")] public double ActualMark { get; set; }
        [JsonPropertyName("maxScore")] public double MaxScore { get; set; }
        [JsonPropertyName("feedback")] public string? Feedback { get; set; }

        /// <summary>Recomputes the percentage from a new raw mark without mutating this entry.</summary>
        public static double ToPercentage(double rawMark, double maxScore)
            => maxScore > 0 ? (rawMark / maxScore) * 100.0 : 0;
    }

    // ─────────────────────────────────────────────────────────────
    //  COURSEWORK MARKS RESPONSE  — the shape returned by
    //  GET /marks/coursework/student/marks (Map<String,Object>)
    //  Backend returns:
    //    { firstname, lastname, form, classroom, Term, Year,
    //      CourseMarks: { "Mathematics": [{ percentageScore, actualMark, maxScore }, ...] } }
    // ─────────────────────────────────────────────────────────────

    public class CourseworkMarksResponse
    {
        [JsonPropertyName("firstname")] public string? Firstname { get; set; }
        [JsonPropertyName("lastname")] public string? Lastname { get; set; }
        [JsonPropertyName("form")] public int? Form { get; set; }
        [JsonPropertyName("classroom")] public string? Classroom { get; set; }
        [JsonPropertyName("Term")] public int? Term { get; set; }
        [JsonPropertyName("Year")] public int? Year { get; set; }
        // Subject name → list of mark entries (each carries percentage, raw, and max)
        [JsonPropertyName("CourseMarks")] public Dictionary<string, List<MarkEntry>> CourseMarks { get; set; } = new();

        /// <summary>Average percentage score per subject (for read-only display).</summary>
        public Dictionary<string, double> AverageBySubject()
        {
            var result = new Dictionary<string, double>();
            foreach (var (subject, entries) in CourseMarks)
            {
                if (entries.Count > 0)
                    result[subject] = entries.Average(e => e.PercentageScore);
            }
            return result;
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  EXAM MARKS RESPONSE  — the shape returned by
    //  GET /marks/exam/student/marks (Map<String,Object>)
    //  Backend returns:
    //    { firstName, lastName, form, classroom, Term, year,
    //      marksBySubject: { "Mathematics": [{score,examId,...}] } }
    // ─────────────────────────────────────────────────────────────

    public class ExamMarksResponse
    {
        [JsonPropertyName("firstName")] public string? Firstname { get; set; }
        [JsonPropertyName("lastName")] public string? Lastname { get; set; }
        [JsonPropertyName("form")] public int? Form { get; set; }
        [JsonPropertyName("classroom")] public string? Classroom { get; set; }
        [JsonPropertyName("Term")] public int? Term { get; set; }
        [JsonPropertyName("year")] public int? Year { get; set; }
        [JsonPropertyName("marksBySubject")] public Dictionary<string, List<ExamMarkDTO>> MarksBySubject { get; set; } = new();
    }

    public class AssignmentModel
    {
        [JsonPropertyName("id")] public long Id { get; set; }
        [JsonPropertyName("subjectId")] public long SubjectId { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("maxScore")] public double MaxScore { get; set; }
        [JsonPropertyName("weight")] public double Weight { get; set; }
    }

    public class AssessmentListItem
    {
        public long Id { get; set; }
        public long SubjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double MaxScore { get; set; }
        public double Weight { get; set; }
        public string Kind { get; set; } = string.Empty;
    }

    public static class OLevelField
    {
        public const string Sciences = "Sciences";
        public const string Commercials = "Commercials";
        public const string Arts = "Arts";
        public static readonly string[] All = { Sciences, Commercials, Arts };
    }

    // ─────────────────────────────────────────────────────────────
    //  GRADING SCALE DTO  (mirrors GradingScaleDTO.java)
    //  Used by GET  /grading-scale/get/{levelType}
    //           POST /grading-scale/set/{levelType}
    // ─────────────────────────────────────────────────────────────

    public class GradingScaleDTO
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("levelType")] public string? LevelType { get; set; }  // "O-Level" or "A-Level"
        [JsonPropertyName("gradeLabel")] public string? GradeLabel { get; set; }  // e.g. "A", "B", "C"
        [JsonPropertyName("minMark")] public double MinMark { get; set; }  // minimum score for this grade
        [JsonPropertyName("points")] public int? Points { get; set; }  // e.g. 5 for A, 4 for B
        [JsonPropertyName("comments")] public string? Comments { get; set; }  // e.g. "Excellent"
    }
}
