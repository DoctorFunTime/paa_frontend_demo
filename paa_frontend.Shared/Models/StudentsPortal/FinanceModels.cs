using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models
{
    public class PaymentRequestDto
    {
        [JsonPropertyName("studentId")] public long StudentId { get; set; }
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("paymentMethod")] public string PaymentMethod { get; set; } = string.Empty;
        [JsonPropertyName("currency")] public string Currency { get; set; } = "USD";
        [JsonPropertyName("mobileNumber")] public string? MobileNumber { get; set; }
        [JsonPropertyName("email")] public string? Email { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("creditStudentAccount")] public bool? CreditStudentAccount { get; set; }
    }

    public class PaymentResponseDto
    {
        [JsonPropertyName("paymentReference")] public string? PaymentReference { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
        [JsonPropertyName("redirectUrl")] public string? RedirectUrl { get; set; }
        [JsonPropertyName("pollUrl")] public string? PollUrl { get; set; }
        [JsonPropertyName("instructions")] public string? Instructions { get; set; }
        [JsonPropertyName("message")] public string? Message { get; set; }
        [JsonPropertyName("amountPaid")] public decimal AmountPaid { get; set; }
        [JsonPropertyName("paidCurrency")] public string? PaidCurrency { get; set; }
        [JsonPropertyName("amountUsd")] public decimal AmountUsd { get; set; }
        [JsonPropertyName("exchangeRateToUsd")] public decimal ExchangeRateToUsd { get; set; }
        [JsonPropertyName("exchangeRateDate")] public string? ExchangeRateDate { get; set; }
    }

    public class BankTransferResponse : PaymentResponseDto
    {
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? BranchCode { get; set; }
        public decimal Amount { get; set; }
    }

    public class AccountSummary
    {
        [JsonPropertyName("studentId")] public long StudentId { get; set; }
        [JsonPropertyName("totalCharged")] public decimal TotalCharged { get; set; }
        [JsonPropertyName("totalPaid")] public decimal TotalPaid { get; set; }
        [JsonPropertyName("balance")] public decimal Balance { get; set; }
        [JsonPropertyName("currency")] public string Currency { get; set; } = "USD";
        [JsonPropertyName("accountStatus")] public string AccountStatus { get; set; } = string.Empty;
        [JsonPropertyName("balanceType")] public string BalanceType { get; set; } = string.Empty;
        [JsonPropertyName("balanceDescription")] public string BalanceDescription { get; set; } = string.Empty;
        [JsonPropertyName("lastPaymentDate")] public string? LastPaymentDate { get; set; }
    }

    public class FinancialStatementDto
    {
        [JsonPropertyName("studentDetails")] public Dictionary<string, object>? StudentDetails { get; set; }
        [JsonPropertyName("accountSummary")] public Dictionary<string, object>? AccountSummary { get; set; }
        [JsonPropertyName("transactions")] public List<Dictionary<string, object>>? Transactions { get; set; }
        [JsonPropertyName("charges")] public List<StatementChargeDto>? Charges { get; set; }
        [JsonPropertyName("generatedAt")] public string? GeneratedAt { get; set; }
        [JsonPropertyName("currency")] public string? Currency { get; set; }
    }

    public class StatementChargeDto
    {
        [JsonPropertyName("invoiceNumber")] public string? InvoiceNumber { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("amountUsd")] public decimal AmountUsd { get; set; }
        [JsonPropertyName("chargedAt")] public string? ChargedAt { get; set; }
        [JsonPropertyName("feeHeadId")] public long? FeeHeadId { get; set; }
    }

    public class InvoiceDto
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("invoiceNumber")] public string? InvoiceNumber { get; set; }
        [JsonPropertyName("studentId")] public string? StudentId { get; set; }
        [JsonPropertyName("issueDate")] public string? IssueDate { get; set; }
        [JsonPropertyName("dueDate")] public string? DueDate { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
        [JsonPropertyName("totalAmount")] public decimal TotalAmount { get; set; }
        [JsonPropertyName("balanceDue")] public decimal BalanceDue { get; set; }
        [JsonPropertyName("currency")] public string Currency { get; set; } = "USD";
        [JsonPropertyName("academicYear")] public string? AcademicYear { get; set; }
        [JsonPropertyName("academicTerm")] public string? AcademicTerm { get; set; }
        [JsonPropertyName("billingCycle")] public string? BillingCycle { get; set; }
        [JsonPropertyName("billingMonth")] public int? BillingMonth { get; set; }
        [JsonPropertyName("accountPosted")] public bool AccountPosted { get; set; }
        [JsonPropertyName("studentName")] public string? StudentName { get; set; }
        [JsonPropertyName("studentGrade")] public string? StudentGrade { get; set; }
        [JsonPropertyName("items")] public List<InvoiceLineItemDto> Items { get; set; } = new();
        [JsonPropertyName("financials")] public InvoiceFinancials? Financials { get; set; }
        [JsonPropertyName("studentDetails")] public InvoiceStudentDetails? StudentDetails { get; set; }
    }

    public class InvoiceFinancials
    {
        [JsonPropertyName("totalAmount")] public decimal TotalAmount { get; set; }
        [JsonPropertyName("amountPaid")] public decimal AmountPaid { get; set; }
        [JsonPropertyName("balanceDue")] public decimal BalanceDue { get; set; }
    }

    public class InvoiceStudentDetails
    {
        [JsonPropertyName("studentId")] public string? StudentId { get; set; }
        [JsonPropertyName("fullName")] public string? FullName { get; set; }
        [JsonPropertyName("currentForm")] public string? CurrentForm { get; set; }
        [JsonPropertyName("stream")] public string? Stream { get; set; }
        [JsonPropertyName("guardianEmail")] public string? GuardianEmail { get; set; }
    }

    public class InvoiceLineItemDto
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("invoiceId")] public long? InvoiceId { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("originalFeeHeadId")] public long? OriginalFeeHeadId { get; set; }
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("discountApplied")] public decimal DiscountApplied { get; set; }
        [JsonPropertyName("finalAmount")] public decimal FinalAmount { get; set; }
        [JsonPropertyName("feeFrequency")] public string? FeeFrequency { get; set; }
        [JsonPropertyName("frequency")] public string? Frequency { get; set; }
    }

    public class FeesStructureDto
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("academicYear")] public string? AcademicYear { get; set; }
        [JsonPropertyName("academicTerm")] public string? AcademicTerm { get; set; }
        [JsonPropertyName("form")] public int Form { get; set; }
        [JsonPropertyName("level")] public string? Level { get; set; }
        [JsonPropertyName("stream")] public string? Stream { get; set; }
        [JsonPropertyName("currency")] public string Currency { get; set; } = "USD";
        [JsonPropertyName("isActive")] public bool IsActive { get; set; }
        [JsonPropertyName("totalAmount")] public decimal TotalAmount { get; set; }
        [JsonPropertyName("feeItems")] public List<FeesStructureItemDto> FeeItems { get; set; } = new();
    }

    public class FeesStructureItemDto
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("feeHead")] public FeesHeadDto? FeeHead { get; set; }
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("isMandatory")] public bool IsMandatory { get; set; }
        [JsonPropertyName("priority")] public int? Priority { get; set; }
    }

    public class FeesHeadDto
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("glCode")] public string? GlCode { get; set; }
        [JsonPropertyName("defaultFrequency")] public string? DefaultFrequency { get; set; }
    }

    public class ScholarshipStudentDto
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("studentId")] public long StudentId { get; set; }
        [JsonPropertyName("scholarshipType")] public string ScholarshipType { get; set; } = string.Empty;
        [JsonPropertyName("scholarshipAmount")] public decimal ScholarshipAmount { get; set; }
    }

    public class StudentAdditionalFeeDto
    {
        public long? Id { get; set; }
        public long StudentId { get; set; }
        public long FeeHeadId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string AcademicTerm { get; set; } = "TERM_1";
        public string BillingCycle { get; set; } = "TERM";
        public int? BillingMonth { get; set; }
        public bool Invoiced { get; set; }
    }

    public class ExchangeRateDto
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("currencyCode")] public string CurrencyCode { get; set; } = string.Empty;
        [JsonPropertyName("rateToUSD")] public decimal RateToUsd { get; set; }
        [JsonPropertyName("effectiveDate")] public string? EffectiveDate { get; set; }
    }

    public class CashFlowRequestDto
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "INCOME";
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("currency")] public string Currency { get; set; } = "USD";
        [JsonPropertyName("category")] public string Category { get; set; } = string.Empty;
        [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
        [JsonPropertyName("paymentMethod")] public string? PaymentMethod { get; set; }
        [JsonPropertyName("counterparty")] public string? Counterparty { get; set; }
        [JsonPropertyName("recordedBy")] public string? RecordedBy { get; set; }
        [JsonPropertyName("occurredOn")] public string? OccurredOn { get; set; }
        [JsonPropertyName("sourceReference")] public string? SourceReference { get; set; }
        [JsonPropertyName("sourceType")] public string? SourceType { get; set; }
    }

    public class CashFlowResponseDto
    {
        [JsonPropertyName("id")] public long? Id { get; set; }
        [JsonPropertyName("transactionReference")] public string? TransactionReference { get; set; }
        [JsonPropertyName("sourceReference")] public string? SourceReference { get; set; }
        [JsonPropertyName("sourceType")] public string? SourceType { get; set; }
        [JsonPropertyName("studentId")] public long? StudentId { get; set; }
        [JsonPropertyName("relatedPaymentReference")] public string? RelatedPaymentReference { get; set; }
        [JsonPropertyName("type")] public string? Type { get; set; }
        [JsonPropertyName("amountUsd")] public decimal AmountUsd { get; set; }
        [JsonPropertyName("currency")] public string? Currency { get; set; }
        [JsonPropertyName("originalAmount")] public decimal OriginalAmount { get; set; }
        [JsonPropertyName("originalCurrency")] public string? OriginalCurrency { get; set; }
        [JsonPropertyName("exchangeRateToUsd")] public decimal ExchangeRateToUsd { get; set; }
        [JsonPropertyName("exchangeRateDate")] public string? ExchangeRateDate { get; set; }
        [JsonPropertyName("category")] public string? Category { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("paymentMethod")] public string? PaymentMethod { get; set; }
        [JsonPropertyName("counterparty")] public string? Counterparty { get; set; }
        [JsonPropertyName("recordedBy")] public string? RecordedBy { get; set; }
        [JsonPropertyName("occurredOn")] public string? OccurredOn { get; set; }
        [JsonPropertyName("createdAt")] public string? CreatedAt { get; set; }
    }

    public class CashFlowSummaryDto
    {
        [JsonPropertyName("cashFeeIncome")] public decimal CashFeeIncome { get; set; }
        [JsonPropertyName("nonCashFeeIncome")] public decimal NonCashFeeIncome { get; set; }
        [JsonPropertyName("otherIncome")] public decimal OtherIncome { get; set; }
        [JsonPropertyName("totalMoneyIn")] public decimal TotalMoneyIn { get; set; }
        [JsonPropertyName("totalMoneyOut")] public decimal TotalMoneyOut { get; set; }
        [JsonPropertyName("netTotal")] public decimal NetTotal { get; set; }
        [JsonPropertyName("currency")] public string Currency { get; set; } = "USD";
        [JsonPropertyName("startDate")] public string? StartDate { get; set; }
        [JsonPropertyName("endDate")] public string? EndDate { get; set; }
        [JsonPropertyName("generatedAt")] public string? GeneratedAt { get; set; }
        [JsonPropertyName("todayIncome")] public decimal TodayIncome { get; set; }
        [JsonPropertyName("todayExpenses")] public decimal TodayExpenses { get; set; }
        [JsonPropertyName("todayNet")] public decimal TodayNet { get; set; }
        [JsonPropertyName("incomeByCategory")] public Dictionary<string, decimal> IncomeByCategory { get; set; } = new();
        [JsonPropertyName("dailySummaries")] public List<CashFlowDailySummaryDto> DailySummaries { get; set; } = new();
    }

    public class CashFlowDailySummaryDto
    {
        [JsonPropertyName("date")] public string? Date { get; set; }
        [JsonPropertyName("income")] public decimal Income { get; set; }
        [JsonPropertyName("expenses")] public decimal Expenses { get; set; }
        [JsonPropertyName("net")] public decimal Net { get; set; }
    }

    public class PaymentRefundRequestDto
    {
        [JsonPropertyName("reason")] public string Reason { get; set; } = string.Empty;
        [JsonPropertyName("paymentMethod")] public string? PaymentMethod { get; set; }
    }

    public class FinancePaymentRecord
    {
        [JsonPropertyName("paymentReference")] public string? PaymentReference { get; set; }
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("currency")] public string? Currency { get; set; }
        [JsonPropertyName("amountPaid")] public decimal AmountPaid { get; set; }
        [JsonPropertyName("paidCurrency")] public string? PaidCurrency { get; set; }
        [JsonPropertyName("amountUsd")] public decimal AmountUsd { get; set; }
        [JsonPropertyName("exchangeRateToUsd")] public decimal ExchangeRateToUsd { get; set; }
        [JsonPropertyName("exchangeRateDate")] public string? ExchangeRateDate { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
        [JsonPropertyName("createdAt")] public string? CreatedAt { get; set; }
        [JsonPropertyName("paidAt")] public string? PaidAt { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("paymentMethod")] public string? PaymentMethod { get; set; }

        public string? Date => PaidAt ?? CreatedAt;
        public string? Reference => PaymentReference;
        public string? PaymentStatus => Status;
        public bool IsConfirmed => Status == "PAID" || Status == "SUCCESS" || Status == "CONFIRMED";
    }

    public class PaymentRecord : FinancePaymentRecord
    {
    }
}
