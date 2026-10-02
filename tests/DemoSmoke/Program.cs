using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using paa_frontend.Client.Services.Demo;
using paa_frontend.Shared.Models;
using static paa_frontend.Shared.Models.StudentProfileModel;
var store = new DemoStore();
using var http = new HttpClient(new DemoHttpHandler(store)) { BaseAddress = new Uri("https://demo.invalid") };
void Check(bool value,string name) { if(!value)throw new Exception(name);Console.WriteLine("PASS " + name); }
foreach(var role in new[]{"admin","teacher","student","burser","super"}) {
var login=await http.PostAsJsonAsync("/api/v1/auth/login",new {username=role,password="demo"});
var json=await login.Content.ReadFromJsonAsync<JsonObject>();Check(login.IsSuccessStatusCode && json?["accessToken"] is not null,"Login "+role);
}
Check((await http.PostAsJsonAsync("/api/v1/auth/login",new {username="admin",password="wrong"})).StatusCode==HttpStatusCode.Unauthorized,"Reject other credentials");
var students=await http.GetFromJsonAsync<JsonObject>("/api/v1/student/get/all?page=0&size=15");Check(students?["content"]?.AsArray().Count==15 && students["last"]?.ToString()=="false","Student pagination");
var student=await http.GetFromJsonAsync<Student>("/api/v1/student/get/1");Check(student?.FirstName=="Alex","Typed student profile");
await http.PatchAsJsonAsync("/api/v1/student/update/1",new {firstName="Edited"});Check((await http.GetFromJsonAsync<Student>("/api/v1/student/get/1"))?.FirstName=="Edited","Local profile edit");
await http.PostAsJsonAsync("/api/v1/student/create",new {firstName="New",lastName="Demo",form=1,classroom="A"});Check(store["student"].Count==25,"Create demo student");
await http.DeleteAsync("/api/v1/student/delete/25");Check(store["student"].Count==24,"Delete demo student");
var reports=await http.GetFromJsonAsync<List<ReportCardModel>>("/api/v1/student-grades/report-card/student/get/1");Check(reports?.Count==1 && reports[0].Subjects.Count==5,"Typed report card");
var marks=await http.PostAsJsonAsync("/api/v1/student-grades/marks/exam/student/get",new {studentId=1,term=3,termYear=2026});Check((await marks.Content.ReadFromJsonAsync<ExamMarksResponse>())?.MarksBySubject.Count==5,"Typed exam marks");
var cw=await http.PostAsJsonAsync("/api/v1/student-grades/marks/coursework/student/get",new {studentId=1,term=3,termYear=2026});Check((await cw.Content.ReadFromJsonAsync<CourseworkMarksResponse>())?.CourseMarks.Count==5,"Typed coursework marks");
var sessions=await http.GetFromJsonAsync<AttendanceSessionListResponse>("/api/v1/attendance/session/date/"+DateTime.Today.ToString("yyyy-MM-dd"));Check(sessions?.Sessions?.Count==1,"Attendance session");
var payment=await http.PostAsJsonAsync("/api/v1/finance/payments/manual",new {studentId=1,amount=50,currency="USD",paymentMethod="CASH"});Check((await payment.Content.ReadFromJsonAsync<PaymentResponseDto>())?.AmountPaid==50,"Simulated payment response");
Check((await http.GetFromJsonAsync<AccountSummary>("/api/v1/finance/accounts/1/summary"))?.Balance==250,"Payment updates balance");
var versions=await http.GetFromJsonAsync<TimetableVersionInfo>("/api/v1/timetables/published/active");Check(versions?.Id==DemoStore.VersionId,"Published timetable");
var periods=await http.GetFromJsonAsync<List<TimetableAssignment>>($"/api/v1/timetables/{versions!.Id}/class/F1%7C1A%7CA");Check(periods?.Count==25,"Class timetable periods");
Check((await http.GetAsync("https://school.invalid/unknown")).StatusCode==HttpStatusCode.UnprocessableEntity,"Unknown requests cannot reach network");
store.Reset();Check((await http.GetFromJsonAsync<Student>("/api/v1/student/get/1"))?.FirstName=="Alex" && (await http.GetFromJsonAsync<AccountSummary>("/api/v1/finance/accounts/1/summary"))?.Balance==300,"Reset restores sample records");
Console.WriteLine("All demo smoke checks passed.");
