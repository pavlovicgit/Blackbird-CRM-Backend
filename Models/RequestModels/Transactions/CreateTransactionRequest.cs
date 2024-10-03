using System;

namespace blackbird_crm.Models.RequestModels.Transactions
{
    public class CreateTransactionRequest
    {
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public decimal DueAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string TransactionStatus { get; set; }
        public string Currency { get; set; }
    }
}
