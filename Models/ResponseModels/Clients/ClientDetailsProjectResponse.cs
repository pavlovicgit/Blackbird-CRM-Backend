namespace blackbird_crm.Models.ResponseModels.Clients
{
    public class ClientDetailsProjectResponse
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
    }
}
