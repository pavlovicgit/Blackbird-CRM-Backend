namespace blackbird_crm.Models.ResponseModels.Clients
{
    public class ClientDetailsTransactionsResponse
    {
        public int Id { get; set; } 
        public int ClientId { get; set; }   
        public decimal Amount { get; set; }
    }
}
