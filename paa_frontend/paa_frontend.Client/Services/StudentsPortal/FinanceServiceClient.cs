using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using paa_frontend.Shared.Models;

namespace paa_frontend.Client.Services.StudentsPortal
{
    public class FinanceServiceClient
    {
        private readonly HttpClient _http;

        public FinanceServiceClient(HttpClient http)
        {
            _http = http;
        }

        public Task<AccountSummary?> GetAccountSummaryAsync(long studentId) =>
            _http.GetFromJsonAsync<AccountSummary>($"/api/v1/finance/accounts/{studentId}/summary");

        public Task<List<AccountSummary>?> GetAllAccountSummariesAsync() =>
            _http.GetFromJsonAsync<List<AccountSummary>>("/api/v1/finance/accounts/all");

        public async Task<InvoiceDto?> GetOrCreateInvoiceAsync(long studentId, string academicYear, string academicTerm)
        {
            // Backend uses monthly billing cycles — resolve the current UTC month and use the monthly endpoint.
            int billingMonth = DateTime.UtcNow.Month;
            var response = await _http.GetAsync($"/api/v1/invoicing/monthly/{studentId}/{academicYear}/{billingMonth}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<InvoiceDto>();
        }

        public async Task<InvoiceDto?> GetOrCreateMonthlyInvoiceAsync(long studentId, string academicYear, int billingMonth)
        {
            var response = await _http.GetAsync($"/api/v1/invoicing/monthly/{studentId}/{academicYear}/{billingMonth}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<InvoiceDto>();
        }

        public async Task<Dictionary<string, object>?> GenerateMonthlyInvoicesAsync(string academicYear, int billingMonth)
        {
            var response = await _http.PostAsync($"/api/v1/invoicing/monthly/generate?academicYear={academicYear}&billingMonth={billingMonth}", null);
            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        }

        public async Task<Dictionary<string, object>?> GenerateTermInvoicesAsync(string academicYear, string academicTerm)
        {
            var response = await _http.PostAsync($"/api/v1/invoicing/term/generate?academicYear={Uri.EscapeDataString(academicYear)}&academicTerm={Uri.EscapeDataString(academicTerm)}", null);
            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        }

        public async Task<Dictionary<string, object>?> GenerateAllInvoicesAsync(string academicYear, string academicTerm, int billingMonth)
        {
            var response = await _http.PostAsync($"/api/v1/invoicing/generate-all?academicYear={Uri.EscapeDataString(academicYear)}&academicTerm={Uri.EscapeDataString(academicTerm)}&billingMonth={billingMonth}", null);
            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        }

        public Task<List<ScholarshipStudentDto>?> GetScholarshipsAsync() =>
            _http.GetFromJsonAsync<List<ScholarshipStudentDto>>("/api/v1/invoicing/scholarships/get/all");

        public async Task<ScholarshipStudentDto?> SaveScholarshipAsync(ScholarshipStudentDto request)
        {
            HttpResponseMessage response = request.Id.HasValue
                ? await _http.PutAsJsonAsync($"/api/v1/invoicing/scholarships/update/{request.Id.Value}", request)
                : await _http.PostAsJsonAsync("/api/v1/invoicing/scholarships/create", request);
            await EnsureSuccessWithDetailsAsync(response);
            var envelope = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
            return envelope != null && envelope.TryGetValue("data", out var data)
                ? data.Deserialize<ScholarshipStudentDto>() : null;
        }

        public async Task DeleteScholarshipAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/invoicing/scholarships/delete/{id}");
            await EnsureSuccessWithDetailsAsync(response);
        }

        public Task<List<PaymentRecord>?> GetPaymentHistoryAsync(long studentId) =>
            _http.GetFromJsonAsync<List<PaymentRecord>>($"/api/v1/finance/payments/history/{studentId}");

        public Task<Dictionary<string, object>?> CheckPaymentStatusAsync(string reference) =>
            _http.GetFromJsonAsync<Dictionary<string, object>>($"/api/v1/finance/payments/status/{reference}");

        public async Task<PaymentResponseDto?> InitiatePaymentAsync(PaymentRequestDto request)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/finance/payments/initiate", request);
            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<PaymentResponseDto>();
        }

        public Task<PaymentResponseDto?> InitiateMobilePaymentAsync(
            long studentId,
            decimal amount,
            string? mobileNumber,
            string? term,
            int termYear,
            string paymentMethod = "PAYNOW_MOBILE")
        {
            return InitiatePaymentAsync(new PaymentRequestDto
            {
                StudentId = studentId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                Currency = "USD",
                MobileNumber = mobileNumber,
                Description = BuildLegacyPaymentDescription(term, termYear, "Mobile payment")
            });
        }

        public async Task<BankTransferResponse?> InitiateBankTransferAsync(
            long studentId,
            decimal amount,
            string? term,
            int termYear)
        {
            var response = await InitiatePaymentAsync(new PaymentRequestDto
            {
                StudentId = studentId,
                Amount = amount,
                PaymentMethod = "BANK_TRANSFER",
                Currency = "USD",
                Description = BuildLegacyPaymentDescription(term, termYear, "Bank transfer")
            });

            if (response is null)
            {
                return null;
            }

            return new BankTransferResponse
            {
                PaymentReference = response.PaymentReference,
                Status = response.Status,
                RedirectUrl = response.RedirectUrl,
                PollUrl = response.PollUrl,
                Instructions = response.Instructions,
                Message = response.Message,
                AmountPaid = response.AmountPaid,
                PaidCurrency = response.PaidCurrency,
                AmountUsd = response.AmountUsd,
                ExchangeRateToUsd = response.ExchangeRateToUsd,
                ExchangeRateDate = response.ExchangeRateDate,
                Amount = amount
            };
        }

        public async Task<PaymentResponseDto?> RecordManualPaymentAsync(
            long studentId, decimal amount, string currency, string paymentMethod, string description)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/finance/payments/manual", new
            {
                studentId,
                amount,
                currency,
                paymentMethod,
                description
            });

            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<PaymentResponseDto>();
        }

        public async Task<AccountSummary?> ChargeStudentAccountAsync(long studentId, decimal amount, string currency, string description)
        {
            var response = await _http.PostAsJsonAsync($"/api/v1/finance/accounts/{studentId}/charge", new
            {
                amount,
                currency,
                description
            });

            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<AccountSummary>();
        }

        public Task<FinancialStatementDto?> GetFinancialStatementAsync(long studentId) =>
            _http.GetFromJsonAsync<FinancialStatementDto>($"/api/v1/finance/statements/{studentId}");

        public Task<CashFlowSummaryDto?> GetCashflowSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = BuildDateQuery(startDate, endDate);
            return _http.GetFromJsonAsync<CashFlowSummaryDto>($"/api/v1/finance/cashflow/summary{query}");
        }

        public Task<List<CashFlowResponseDto>?> GetCashflowMovementsAsync(DateTime? startDate = null, DateTime? endDate = null, string? type = null)
        {
            var parts = new List<string>();
            if (startDate.HasValue) parts.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) parts.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            if (!string.IsNullOrWhiteSpace(type)) parts.Add($"type={Uri.EscapeDataString(type)}");
            var query = parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
            return _http.GetFromJsonAsync<List<CashFlowResponseDto>>($"/api/v1/finance/cashflow/movements{query}");
        }

        public async Task<CashFlowResponseDto?> RecordCashflowMovementAsync(CashFlowRequestDto request)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/finance/cashflow/movements", request);
            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<CashFlowResponseDto>();
        }

        public async Task<PaymentResponseDto?> RefundPaymentAsync(string paymentReference, PaymentRefundRequestDto request)
        {
            var response = await _http.PostAsJsonAsync($"/api/v1/finance/payments/{Uri.EscapeDataString(paymentReference)}/refund", request);
            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<PaymentResponseDto>();
        }

        public Task<List<ExchangeRateDto>?> GetExchangeRatesAsync() =>
            _http.GetFromJsonAsync<List<ExchangeRateDto>>("/api/v1/finance/exchange-rates");

        public async Task<ExchangeRateDto?> SaveExchangeRateAsync(ExchangeRateDto request)
        {
            var response = request.CurrencyCode?.Length > 0
                ? await _http.PutAsJsonAsync($"/api/v1/finance/exchange-rates/{request.CurrencyCode}", request)
                : await _http.PostAsJsonAsync("/api/v1/finance/exchange-rates", request);

            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<ExchangeRateDto>();
        }

        public async Task<Dictionary<string, object>?> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            var query = $"?amount={amount}&from={Uri.EscapeDataString(fromCurrency)}&to={Uri.EscapeDataString(toCurrency)}";
            return await _http.GetFromJsonAsync<Dictionary<string, object>>($"/api/v1/finance/exchange-rates/convert{query}");
        }

        public Task<List<FeesHeadDto>?> GetFeeHeadsAsync() =>
            _http.GetFromJsonAsync<List<FeesHeadDto>>("/api/v1/fees-configuration/fees-heads");

        public async Task SaveFeeHeadAsync(FeesHeadDto request)
        {
            HttpResponseMessage response;
            if (request.Id.HasValue && request.Id.Value > 0)
            {
                response = await _http.PutAsJsonAsync($"/api/v1/fees-configuration/fees-heads/{request.Id.Value}", request);
            }
            else
            {
                response = await _http.PostAsJsonAsync("/api/v1/fees-configuration/fees-heads", request);
            }

            await EnsureSuccessWithDetailsAsync(response);
        }

        public async Task DeleteFeeHeadAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/fees-configuration/fees-heads/{id}");
            await EnsureSuccessWithDetailsAsync(response);
        }

        public Task<List<FeesStructureDto>?> GetFeeStructuresAsync() =>
            _http.GetFromJsonAsync<List<FeesStructureDto>>("/api/v1/fees-configuration/structures/fees-structures/all");

        public Task<FeesStructureDto?> GetFeeStructureAsync(long id) =>
            _http.GetFromJsonAsync<FeesStructureDto>($"/api/v1/fees-configuration/structures/{id}");

        public async Task<FeesStructureDto?> SaveFeeStructureAsync(FeesStructureDto request)
        {
            HttpResponseMessage response;
            if (request.Id.HasValue && request.Id.Value > 0)
            {
                response = await _http.PutAsJsonAsync($"/api/v1/fees-configuration/structures/{request.Id.Value}", request);
            }
            else
            {
                response = await _http.PostAsJsonAsync("/api/v1/fees-configuration/structures/create/fees-structure", request);
            }

            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<FeesStructureDto>();
        }

        public async Task SetFeeStructureStatusAsync(long id, bool isActive)
        {
            var response = await _http.PatchAsync($"/api/v1/fees-configuration/structures/{id}/status?isActive={isActive.ToString().ToLowerInvariant()}", null);
            await EnsureSuccessWithDetailsAsync(response);
        }

        public async Task DeleteFeeStructureAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/fees-configuration/structures/{id}");
            await EnsureSuccessWithDetailsAsync(response);
        }

        public Task<List<FeesStructureItemDto>?> GetFeeStructureItemsAsync(long structureId) =>
            _http.GetFromJsonAsync<List<FeesStructureItemDto>>($"/api/v1/fees-configuration/items/structure/{structureId}");

        public async Task<FeesStructureItemDto?> AddFeeStructureItemAsync(long feeStructureId, long feeHeadId, decimal amount, bool isMandatory, int? priority)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/fees-configuration/items", new
            {
                feeStructureId,
                feeHeadId,
                amount,
                isMandatory,
                priority
            });

            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<FeesStructureItemDto>();
        }

        public async Task<FeesStructureItemDto?> UpdateFeeStructureItemAsync(long itemId, decimal amount, bool isMandatory, int? priority)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/fees-configuration/items/{itemId}", new
            {
                amount,
                isMandatory,
                priority
            });

            await EnsureSuccessWithDetailsAsync(response);
            return await response.Content.ReadFromJsonAsync<FeesStructureItemDto>();
        }

        public async Task DeleteFeeStructureItemAsync(long itemId)
        {
            var response = await _http.DeleteAsync($"/api/v1/fees-configuration/items/{itemId}");
            await EnsureSuccessWithDetailsAsync(response);
        }

        private static string BuildDateQuery(DateTime? startDate, DateTime? endDate)
        {
            var parts = new List<string>();
            if (startDate.HasValue) parts.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) parts.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
        }

        private static string BuildLegacyPaymentDescription(string? term, int termYear, string fallback)
        {
            if (!string.IsNullOrWhiteSpace(term))
            {
                return $"{term} {termYear} payment";
            }

            return fallback;
        }

        private static async Task EnsureSuccessWithDetailsAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            var message = string.IsNullOrWhiteSpace(errorBody)
                ? $"Request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase})."
                : errorBody;

            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
