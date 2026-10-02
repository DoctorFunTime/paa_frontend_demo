using System.Net.Http.Json;
using paa_frontend.Shared.Models.Finance;

namespace paa_frontend.Client.Services.StudentPass
{
    /// <summary>
    /// Typed HTTP client for paa_finance_service StudentPass endpoints.
    /// Base URL should be registered as "FinanceService" in Program.cs, e.g.:
    ///   builder.Services.AddHttpClient<FinancePassServiceClient>(
    ///       c => c.BaseAddress = new Uri(builder.Configuration["Services:Finance"]!));
    /// </summary>
    public class FinancePassServiceClient
    {
        private readonly HttpClient _http;

        public FinancePassServiceClient(HttpClient http) => _http = http;

        // ── Account lookup ────────────────────────────────────────────────────

        /// <summary>GET /api/v1/finance/student-pass/account/{nfcId}</summary>
        public async Task<FinanceAccountDto?> GetAccountByNfcIdAsync(string nfcId)
        {
            var cleanNfcId = nfcId.Replace(":", "").Trim();
            var response = await _http.GetAsync(
                $"/api/v1/finance/student-pass/account/{Uri.EscapeDataString(cleanNfcId)}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FinanceAccountDto>();
        }

        /// <summary>GET /api/v1/finance/accounts/all</summary>
        public async Task<List<FinanceAccountDto>?> GetAllAccountsAsync()
        {
            var response = await _http.GetAsync("/api/v1/finance/accounts/all");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<FinanceAccountDto>>();
        }

        // ── Gate validation ───────────────────────────────────────────────────

        /// <summary>GET /api/v1/finance/student-pass/validate/{nfcId}</summary>
        public async Task<FinanceValidationResult?> ValidateCardAsync(string nfcId)
        {
            var cleanNfcId = nfcId.Replace(":", "").Trim();
            var response = await _http.GetAsync(
                $"/api/v1/finance/student-pass/validate/{Uri.EscapeDataString(cleanNfcId)}");

            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<FinanceValidationResult>();
        }

        // ── NFC assignment ────────────────────────────────────────────────────

        /// <summary>PATCH /api/v1/finance/student-pass/account/nfc/set/{studentId}</summary>
        public async Task<bool> SetNfcValueAsync(long studentId, string nfcId)
        {
            var cleanNfcId = nfcId.Replace(":", "").Trim();
            var response = await _http.PatchAsJsonAsync(
                $"/api/v1/finance/student-pass/account/nfc/set/{studentId}",
                new { nfcId = cleanNfcId });
            return response.IsSuccessStatusCode;
        }

        // ── Special passes ────────────────────────────────────────────────────

        /// <summary>POST /api/v1/finance/student-pass/special-pass/create</summary>
        public async Task<bool> CreateSpecialPassAsync(SpecialPassRequest request)
        {
            request.NfcId = request.NfcId.Replace(":", "").Trim();
            var response = await _http.PostAsJsonAsync(
                "/api/v1/finance/student-pass/special-pass/create", request);
            return response.IsSuccessStatusCode;
        }

        /// <summary>PUT /api/v1/finance/student-pass/special-pass/edit</summary>
        public async Task<bool> EditSpecialPassAsync(SpecialPassRequest request)
        {
            request.NfcId = request.NfcId.Replace(":", "").Trim();
            var response = await _http.PutAsJsonAsync(
                "/api/v1/finance/student-pass/special-pass/edit", request);
            return response.IsSuccessStatusCode;
        }

        /// <summary>DELETE /api/v1/finance/student-pass/special-pass/delete/{nfcId}</summary>
        public async Task<bool> DeleteSpecialPassAsync(string nfcId)
        {
            var cleanNfcId = nfcId.Replace(":", "").Trim();
            var response = await _http.DeleteAsync(
                $"/api/v1/finance/student-pass/special-pass/delete/{Uri.EscapeDataString(cleanNfcId)}");
            return response.IsSuccessStatusCode;
        }
    }
}

// ── Shared models (add to paa_frontend.Shared/Models/Finance/) ────────────────

namespace paa_frontend.Shared.Models.Finance
{
    public class FinanceValidationResult
    {
        public long? StudentId { get; set; }
        public bool Allowed { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class FinanceAccountDto
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public decimal TotalCharged { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }
        public string AccountStatus { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public string? NfcId { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SpecialPassRequest
    {
        public string NfcId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
    }
}
