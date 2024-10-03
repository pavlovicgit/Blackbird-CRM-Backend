namespace blackbird_crm.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public decimal DueAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string TransactionStatus { get; set; }
        public string Currency { get; set; }
        public Client Client { get; set; }
        public Project Project { get; set; }
    }
}
