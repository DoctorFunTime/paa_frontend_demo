using System.Text.Json;
using System.Text.Json.Nodes;

namespace paa_frontend.Client.Services.Demo;

/// <summary>Fictional browser-memory data. No disk, school API, or payment provider.</summary>
public sealed class DemoStore
{
    public Dictionary<string, List<JsonObject>> Tables { get; } = new();
    public static readonly string[] Subjects = ["Mathematics", "English", "Biology", "Geography", "History"];
    public static readonly Guid VersionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public List<JsonObject> this[string key] => Tables.TryGetValue(key, out var table) ? table : Tables[key] = new();
    public static JsonObject Row(object value) => JsonSerializer.SerializeToNode(value)!.AsObject();
    public static string Text(JsonNode? row, string key) => row?[key]?.ToString() ?? "";
    public static int Number(JsonNode? row, string key) => int.TryParse(Text(row, key), out var n) ? n : 0;
    public DemoStore() => Reset();
    public void Reset()
    {
        Tables.Clear();
        var today = DateTime.Today;
        var date = today.ToString("yyyy-MM-dd");
        var term = $"Term {(today.Month + 3) / 4}";
        string[] firstNames = ["Alex", "Jamie", "Taylor", "Morgan", "Jordan", "Casey", "Riley", "Avery", "Sam", "Robin", "Cameron", "Drew"];
        for (var i = 1; i <= 24; i++)
        {
            var form = (i - 1) / 4 + 1;
            this["student"].Add(Row(new { id = i, firstName = firstNames[(i - 1) % 12], lastName = $"Demo{i:00}", email = $"student{i}@example.invalid", phone = "0000000000", address = "Demo address", dateOfBirth = $"{today.Year - 12 - form}-03-15T00:00:00", classroom = "A", form, gender = i % 2 == 0 ? "Female" : "Male", nationalId = $"DEMO-{i:000}", tempId = $"DEMO-{i:000}", status = "Active", createdAt = $"{today.Year}-01-10T00:00:00", emergencyContactDetails = new { name = "Demo Guardian", phone = "0000000000", email = "guardian@example.invalid", relationship = "Guardian" } }));
            this["accounts"].Add(Row(new { id = i, studentId = i, totalCharged = 900, totalPaid = i % 3 == 0 ? 900 : 600, balance = i % 3 == 0 ? 0 : 300, currency = "USD", accountStatus = "ACTIVE", balanceType = "DEBIT", balanceDescription = "Demo account", lastPaymentDate = date, nfcId = $"DEMO{i:000}" }));
            this["payments"].Add(Row(new { id = i, studentId = i, paymentReference = $"DEMO-PAY-{i:000}", amount = i % 3 == 0 ? 900 : 600, amountPaid = i % 3 == 0 ? 900 : 600, amountUsd = i % 3 == 0 ? 900 : 600, currency = "USD", paidCurrency = "USD", status = "PAID", paymentMethod = "CASH", paymentDate = date, createdAt = date, description = "Sample payment — no money transferred" }));
            this["reports"].Add(Row(new { id = i, studentId = i, term, termYear = today.Year, termMonth = today.ToString("MMMM"), generatedDate = date, courseworkWeight = .3, studentFirstName = firstNames[(i - 1) % 12], studentLastName = $"Demo{i:00}", studentForm = form, studentClass = "A", teacherSubmitted = true, adminApproved = i % 2 == 1, releasedToStudent = i % 2 == 1, overallComments = "Sample report for demonstration only.", subjects = Subjects.Select((s,j) => new { subjectName = s, examMark = 70+j*3, courseworkMark = 80, finalMark = 73+j*2, grade = j>2 ? "A" : "B", comments = "Good progress in this sample report." }) }));
        }
        foreach (var kind in new[] { "admin", "teacher", "burser" })
            for (int i = 1; i <= (kind == "teacher" ? 5 : 1); i++)
                this[kind].Add(Row(new { id = i, firstName = kind == "teacher" ? firstNames[i] : "Demo", lastName = kind == "teacher" ? "Teacher" : kind == "admin" ? "Administrator" : "Bursar", email = $"{kind}{i}@example.invalid", phone = "0000000000", gender = "Other", nationalId = $"DEMO-{kind}-{i}", dateOfBirth = "1990-01-01T00:00:00", status = "Active" }));
        foreach (var level in new[] { "olevel", "alevel" })
            this[level + "-classes"].Add(Row(new { id = 1, className = "A" }));
        for (int f = 1; f <= 6; f++)
            this["class-subjects"].Add(Row(new { id = f, form = f, className = "A", classCode = $"{f}A", subjects = Subjects }));
        for (int j = 0; j < Subjects.Length; j++)
        {
            this["subjects"].Add(Row(new { id = j+1, subjectName = Subjects[j], subject = Subjects[j], field = "Sciences" }));
            foreach (var type in new[] { "assignment", "test", "exam" })
                this[type].Add(Row(new { id = j+1, subjectId = j+1, subjectName = Subjects[j], title = $"{Subjects[j]} demo {type}", maxScore = 100, weight = .2, dueDate = today.AddDays(7).ToString("yyyy-MM-dd"), testDate = date, examDate = date, teacherId = j+1, level = "form1to4", form = 1, className = "A", term, termYear = today.Year }));
        }
        this["fields"].Add(Row(new { id = 1, field = "Sciences" }));
        foreach (var level in new[] { "O-Level", "A-Level" })
            foreach (var (grade, mark) in new[] { ("A",80), ("B",65), ("C",50), ("D",40), ("E",30), ("U",0) })
                this["grading"].Add(Row(new { id = this["grading"].Count+1, levelType = level, gradeLabel = grade, minMark = mark, points = mark/15, comments = "Demo grading scale" }));
        this["sessions"].Add(Row(new { id = 1, date, type = "Student", referenceId = 1, createdBy = 1 }));
        foreach (var student in this["student"])
            this["records"].Add(Row(new { id = Number(student,"id"), sessionId = 1, personId = Number(student,"id"), status = Number(student,"id") % 8 == 0 ? "Absent" : "Present", remarks = "Sample attendance" }));
        this["fees-heads"].Add(Row(new { id = 1, name = "Tuition", description = "Demo tuition", glCode = "DEMO-100", defaultFrequency = "TERMLY" }));
        this["fee-items"].Add(Row(new { id = 1, feeStructureId = 1, feeHead = this["fees-heads"][0], amount = 300, isMandatory = true, priority = 1 }));
        this["fee-structures"].Add(Row(new { id = 1, name = "Demo fee structure", academicYear = today.Year.ToString(), academicTerm = term, form = 1, level = "O-Level", stream = "A", currency = "USD", isActive = true, totalAmount = 300, feeItems = this["fee-items"] }));
        this["rates"].Add(Row(new { id = 1, currencyCode = "USD", rateToUSD = 1, effectiveDate = date }));
        this["cashflow"].Add(Row(new { id = 1, transactionReference = "DEMO-CF-001", type = "INCOME", amountUsd = 250, originalAmount = 250, currency = "USD", category = "Other income", description = "Fictional income", occurredOn = date, createdAt = date, paymentMethod = "CASH" }));
        this["versions"].Add(Row(new { id = VersionId, name = "Demo timetable", term = $"{today.Year}-T{(today.Month+3)/4}", lifecycle = "PUBLISHED", createdAt = DateTimeOffset.Now }));
        for (int day = 1; day <= 5; day++)
            for (int period = 1; period <= 5; period++)
            {
                var id = Guid.NewGuid();
                this["slots"].Add(Row(new { id, dayOfWeek = day, periodNumber = period, startTime = $"{7+period:00}:00:00", endTime = $"{7+period:00}:45:00", type = "LESSON", blocked = false }));
                this["periods"].Add(Row(new { id = Guid.NewGuid(), versionId = VersionId, classId = "F1|1A|A", subjectId = Subjects[period-1], teacherId = period.ToString(), roomId = "DEMO-1", slotId = id, locked = false, source = "DEMO", countsTowardAllocation = true }));
            }
        foreach (var s in Subjects.Select((name,i) => new { name, i }))
        {
            this["allocations"].Add(Row(new { id = Guid.NewGuid(), classId = "F1|1A|A", subjectId = s.name, term = $"{today.Year}-T{(today.Month+3)/4}", plannedPeriods = 5, doublePeriodsPerWeek = 0, teacherId = (s.i+1).ToString() }));
            this["teacher-subjects"].Add(Row(new { id = Guid.NewGuid(), teacherId = (s.i+1).ToString(), subjectId = s.name, teacherName = firstNames[s.i+1]+" Teacher" }));
        }
        this["rooms"].Add(Row(new { id = Guid.NewGuid(), roomCode = "DEMO-1", roomType = "CLASSROOM", capacity = 30, active = true }));
    }
}
