using System.ComponentModel.DataAnnotations;

namespace paa_frontend.Shared.Models;

public sealed class StudentApplicationSubmission
{
    [Required, StringLength(100)] public string FirstName { get; set; } = "";
    [Required, StringLength(100)] public string LastName { get; set; } = "";
    [Required, StringLength(10)] public string Gender { get; set; } = "";
    [EmailAddress, StringLength(50)] public string? Email { get; set; }
    [Required, StringLength(10, MinimumLength = 7)] public string Phone { get; set; } = "";
    [Required] public DateTime? DateOfBirth { get; set; }
    [Required, StringLength(100)] public string Address { get; set; } = "";
    [Required, Range(1, 6)] public int? Form { get; set; }
    [StringLength(150)] public string? PreviousSchool { get; set; }
    [Range(0, 30)] public int? SubjectsPassed { get; set; }
    [Range(4, 36)] public int? GradeSevenUnits { get; set; }
    [StringLength(1000)] public string? SubjectsStudied { get; set; }
    [Required, StringLength(100)] public string Religion { get; set; } = "";
    [StringLength(100)] public string? NationalId { get; set; }
    [StringLength(100)] public string? GuardianName { get; set; }
    [StringLength(10)] public string? GuardianPhone { get; set; }
    [EmailAddress, StringLength(50)] public string? GuardianEmail { get; set; }
    [StringLength(10)] public string? GuardianRelationship { get; set; }
}
