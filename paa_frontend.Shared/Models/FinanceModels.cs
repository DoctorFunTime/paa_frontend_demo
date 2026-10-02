using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models
{
    public class FinanceModels
    {
        public class StudentFinancialProfile
        {
            [JsonPropertyName("id")] public int Id { get; set; }
            [JsonPropertyName("firstName")] public string? FirstName { get; set; }
            [JsonPropertyName("lastName")] public string? LastName { get; set; }
            [JsonPropertyName("nationalId")] public string? NationalId { get; set; }
            [JsonPropertyName("form")] public int? Form { get; set; }
            [JsonPropertyName("classroom")] public string? Classroom { get; set; }
            [JsonPropertyName("balanceOwing")] public decimal BalanceOwing { get; set; }
            [JsonPropertyName("totalPaid")] public decimal TotalPaid { get; set; }
            [JsonPropertyName("status")] public string? Status { get; set; }
            
            [JsonIgnore] public string FullName => $"{FirstName} {LastName}";
        }

        public class TransactionDto
        {
            [JsonPropertyName("id")] public int Id { get; set; }
            [JsonPropertyName("studentId")] public int StudentId { get; set; }
            [JsonPropertyName("documentNumber")] public string? DocumentNumber { get; set; }
            [JsonPropertyName("transactionDate")] public DateTime TransactionDate { get; set; }
            [JsonPropertyName("accountType")] public string? AccountType { get; set; } // School Fees, Receipt, Service Exchange
            [JsonPropertyName("amount")] public decimal Amount { get; set; }
            [JsonPropertyName("tenderedAmount")] public decimal? TenderedAmount { get; set; }
            [JsonPropertyName("changeAmount")] public decimal? ChangeAmount { get; set; }
            [JsonPropertyName("currencyCode")] public string? CurrencyCode { get; set; } // USD, ZIG
            [JsonPropertyName("paymentType")] public string? PaymentType { get; set; } // Cash, Ecocash, Innbucks, Bank Transfer
            [JsonPropertyName("terms")] public string? Terms { get; set; }
            [JsonPropertyName("description")] public string? Description { get; set; }
            [JsonPropertyName("isAdjustment")] public bool IsAdjustment { get; set; }
            [JsonPropertyName("adjustmentType")] public string? AdjustmentType { get; set; }
            
            // Payment Provider Specific Fields
            [JsonPropertyName("phoneNumber")] public string? PhoneNumber { get; set; }
            [JsonPropertyName("referenceNumber")] public string? ReferenceNumber { get; set; }
            [JsonPropertyName("bankAccountDetails")] public string? BankAccountDetails { get; set; }
            
            [JsonPropertyName("lineItems")] public List<TransactionLineItem> LineItems { get; set; } = new();
        }

        public class TransactionLineItem
        {
            [JsonPropertyName("id")] public int Id { get; set; }
            [JsonPropertyName("description")] public string? Description { get; set; }
            [JsonPropertyName("amount")] public decimal Amount { get; set; }
            [JsonPropertyName("isSelected")] public bool IsSelected { get; set; }
        }

        public class ReceiptDetails
        {
            [JsonPropertyName("receiptNumber")] public string? ReceiptNumber { get; set; }
            [JsonPropertyName("date")] public DateTime Date { get; set; }
            [JsonPropertyName("studentName")] public string? StudentName { get; set; }
            [JsonPropertyName("studentForm")] public string? StudentForm { get; set; }
            [JsonPropertyName("paymentMethod")] public string? PaymentMethod { get; set; }
            [JsonPropertyName("amountPaid")] public decimal AmountPaid { get; set; }
            [JsonPropertyName("currency")] public string? Currency { get; set; }
            [JsonPropertyName("balanceRemaining")] public decimal BalanceRemaining { get; set; }
            [JsonPropertyName("items")] public List<TransactionLineItem> Items { get; set; } = new();
        }

        public class StatementEntry
        {
            [JsonPropertyName("date")] public DateTime Date { get; set; }
            [JsonPropertyName("reference")] public string? Reference { get; set; }
            [JsonPropertyName("description")] public string? Description { get; set; }
            [JsonPropertyName("debit")] public decimal? Debit { get; set; }
            [JsonPropertyName("credit")] public decimal? Credit { get; set; }
            [JsonPropertyName("runningBalance")] public decimal RunningBalance { get; set; }
        }

        public class CashbookEntry
        {
            [JsonPropertyName("date")] public DateTime Date { get; set; }
            [JsonPropertyName("details")] public string? Details { get; set; }
            [JsonPropertyName("reference")] public string? Reference { get; set; }
            [JsonPropertyName("cashIn")] public decimal? CashIn { get; set; }
            [JsonPropertyName("cashOut")] public decimal? CashOut { get; set; }
            [JsonPropertyName("bankIn")] public decimal? BankIn { get; set; }
            [JsonPropertyName("bankOut")] public decimal? BankOut { get; set; }
            [JsonPropertyName("balance")] public decimal Balance { get; set; }
        }

        public class BurserDashboardData
        {
            [JsonPropertyName("totalOwed")] public decimal TotalOwed { get; set; }
            [JsonPropertyName("totalReceived")] public decimal TotalReceived { get; set; }
            [JsonPropertyName("totalExpenses")] public decimal TotalExpenses { get; set; }
            [JsonPropertyName("activeDebtorsCount")] public int ActiveDebtorsCount { get; set; }
            [JsonPropertyName("collectionRate")] public double CollectionRate { get; set; } // Percentage
            
            // For charts
            [JsonPropertyName("monthlyRevenue")] public List<ChartDataPoint> MonthlyRevenue { get; set; } = new();
            [JsonPropertyName("monthlyExpenses")] public List<ChartDataPoint> MonthlyExpenses { get; set; } = new();
        }

        public class ChartDataPoint
        {
            [JsonPropertyName("label")] public string? Label { get; set; }
            [JsonPropertyName("value")] public decimal Value { get; set; }
        }
    }
}
