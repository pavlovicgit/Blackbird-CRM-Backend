namespace blackbird_crm.Models.ResponseModels.Clients
{
    public class ClientDetailsResponse
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public List<ClientDetailsProjectResponse> Projects { get; set; }
        public List<ClientDetailsCommentsResponse> Comments { get; set; }
        public List<ClientDetailsTransactionsResponse> Transaction { get; set; }
    }
}
