using System.Net.Http.Json; using paa_frontend.Shared.Models;
namespace paa_frontend.Client.Services;
public sealed class TimetableServiceClient
{
    private readonly HttpClient _http;
    public TimetableServiceClient(HttpClient http)=>_http=http;
    public async Task<List<TimetableAssignment>> GetByClassAsync(Guid version,string id)=>await _http.GetFromJsonAsync<List<TimetableAssignment>>($"/api/v1/timetables/{version}/class/{Uri.EscapeDataString(id)}")??new();
    public async Task<List<TimetableAssignment>> GetBySubjectAsync(Guid version,string id)=>await _http.GetFromJsonAsync<List<TimetableAssignment>>($"/api/v1/timetables/{version}/subject/{Uri.EscapeDataString(id)}")??new();
    public async Task<List<TimetableAssignment>> GetByTeacherAsync(Guid version,string id)=>await _http.GetFromJsonAsync<List<TimetableAssignment>>($"/api/v1/timetables/{version}/teacher/{Uri.EscapeDataString(id)}")??new();
    public async Task<TimetableVersionInfo?> GetActivePublishedAsync(){using var response=await _http.GetAsync("/api/v1/timetables/published/active");if(response.StatusCode==System.Net.HttpStatusCode.NoContent)return null;await EnsureSuccess(response);return await response.Content.ReadFromJsonAsync<TimetableVersionInfo>();}
    public async Task<List<TimetableVersionInfo>> GetVersionsAsync(string term)=>await _http.GetFromJsonAsync<List<TimetableVersionInfo>>($"/api/v1/timetables/versions?term={Uri.EscapeDataString(term)}")??new();
    public async Task<TimetableVersionInfo?> PublishAsync(Guid version){var x=await _http.PostAsync($"/api/v1/timetables/{version}/publish",null);await EnsureSuccess(x);return await x.Content.ReadFromJsonAsync<TimetableVersionInfo>();}
    public async Task<TimetableVersionInfo?> EditDraftAsync(Guid version){using var response=await _http.PostAsync($"/api/v1/timetables/{version}/draft",null);await EnsureSuccess(response);return await response.Content.ReadFromJsonAsync<TimetableVersionInfo>();}
    public async Task DeleteVersionAsync(Guid version){using var response=await _http.DeleteAsync($"/api/v1/timetables/{version}");await EnsureSuccess(response);}
    public async Task<TimetableJob?> GenerateAsync(TimetableGenerateRequest r){var x=await _http.PostAsJsonAsync("/api/v1/timetables/generate",r);await EnsureSuccess(x);return await x.Content.ReadFromJsonAsync<TimetableJob>();}
    public async Task<TimetableJob?> GetJobAsync(Guid id)=>await _http.GetFromJsonAsync<TimetableJob>($"/api/v1/timetables/jobs/{id}");
    public async Task<List<TimetableGenerationIssue>> ValidateAsync(TimetableGenerateRequest r){var x=await _http.PostAsJsonAsync("/api/v1/timetables/validate",r);await EnsureSuccess(x);return await x.Content.ReadFromJsonAsync<List<TimetableGenerationIssue>>()??new();}
    public async Task<TimetableAssignment?> PlaceAsync(TimetablePeriodRequest r){var x=await _http.PostAsJsonAsync($"/api/v1/timetables/{r.VersionId}/periods",r);await EnsureSuccess(x);return await x.Content.ReadFromJsonAsync<TimetableAssignment>();}
    public async Task<TimetableAssignment?> MovePeriodAsync(Guid assignmentId,Guid slotId){var x=await _http.PutAsJsonAsync($"/api/v1/periods/{assignmentId}/move",new TimetableMovePeriodRequest{SlotId=slotId});await EnsureSuccess(x);return await x.Content.ReadFromJsonAsync<TimetableAssignment>();}
    public async Task<List<TimetableSlot>> GetSlotsAsync()=>await _http.GetFromJsonAsync<List<TimetableSlot>>("/api/v1/timetables/config/slots")??new();
    public async Task<TimetableSlot?> SaveSlotAsync(TimetableSlotRequest r){var x=await _http.PostAsJsonAsync("/api/v1/timetables/config/slots",r);await EnsureSuccess(x);return await x.Content.ReadFromJsonAsync<TimetableSlot>();}
    public async Task DeleteSlotAsync(Guid id){var x=await _http.DeleteAsync($"/api/v1/timetables/config/slots/{id}");await EnsureSuccess(x);}
    public async Task<List<TimetableAllocation>> GetAllocationsAsync(string term)=>await _http.GetFromJsonAsync<List<TimetableAllocation>>($"/api/v1/timetables/config/allocations?term={Uri.EscapeDataString(term)}")??new();
    public async Task<TimetableAllocation?> SaveAllocationAsync(TimetableAllocation r){var x=await _http.PostAsJsonAsync("/api/v1/timetables/config/allocations",r);await EnsureSuccess(x);return await x.Content.ReadFromJsonAsync<TimetableAllocation>();}
    public async Task DeleteAllocationAsync(Guid id){var x=await _http.DeleteAsync($"/api/v1/timetables/config/allocations/{id}");await EnsureSuccess(x);}
    public async Task<List<TimetableTeacherSubject>> GetTeacherSubjectsAsync()=>await _http.GetFromJsonAsync<List<TimetableTeacherSubject>>("/api/v1/timetables/config/teachers/subjects")??new();
    public async Task<TimetableTeacherSubject?> SaveTeacherSubjectAsync(TimetableTeacherSubjectRequest r){var x=await _http.PostAsJsonAsync("/api/v1/timetables/config/teachers/subjects",r);await EnsureSuccess(x);return await x.Content.ReadFromJsonAsync<TimetableTeacherSubject>();}
    public async Task DeleteTeacherSubjectAsync(Guid id){var x=await _http.DeleteAsync($"/api/v1/timetables/config/teachers/subjects/{id}");await EnsureSuccess(x);}
    public async Task<List<TimetableTeacherUnavailable>> GetTeacherUnavailabilityAsync()=>await _http.GetFromJsonAsync<List<TimetableTeacherUnavailable>>("/api/v1/timetables/config/teacher-unavailability")??new();
    public async Task<TimetableTeacherUnavailable?> SetTeacherUnavailableAsync(TimetableTeacherUnavailableRequest r){var x=await _http.PostAsJsonAsync("/api/v1/timetables/config/teacher-unavailability",r);await EnsureSuccess(x);return await x.Content.ReadFromJsonAsync<TimetableTeacherUnavailable>();}
    public async Task DeleteTeacherUnavailableAsync(string teacherId,Guid slotId){var x=await _http.DeleteAsync($"/api/v1/timetables/config/teacher-unavailability/{Uri.EscapeDataString(teacherId)}/{slotId}");await EnsureSuccess(x);}
    public async Task<List<TimetableRoom>> GetRoomsAsync()=>await _http.GetFromJsonAsync<List<TimetableRoom>>("/api/v1/timetables/config/rooms")??new();
    public async Task<TimetableRoom?> SaveRoomAsync(TimetableRoomRequest r){var x=await _http.PostAsJsonAsync("/api/v1/timetables/config/rooms",r);await EnsureSuccess(x);return await x.Content.ReadFromJsonAsync<TimetableRoom>();}
    public async Task DeleteRoomAsync(Guid id){var x=await _http.DeleteAsync($"/api/v1/timetables/config/rooms/{id}");await EnsureSuccess(x);}
    private static async Task EnsureSuccess(HttpResponseMessage response){if(response.IsSuccessStatusCode)return;var detail=await response.Content.ReadAsStringAsync();throw new InvalidOperationException(string.IsNullOrWhiteSpace(detail)?$"Timetable API returned {(int)response.StatusCode} {response.ReasonPhrase}":detail);}
}
