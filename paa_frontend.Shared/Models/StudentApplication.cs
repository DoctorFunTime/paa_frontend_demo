namespace paa_frontend.Shared.Models;

public class StudentApplication
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public int? Form { get; set; }
    public string? PreviousSchool { get; set; }
    public int? SubjectsPassed { get; set; }
    public int? GradeSevenUnits { get; set; }
    public string? SubjectsStudied { get; set; }
    public string? Religion { get; set; }
    public string? NationalId { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? GuardianEmail { get; set; }
    public string? GuardianRelationship { get; set; }
    public bool? Paid { get; set; }
    public string? Status { get; set; }
}
