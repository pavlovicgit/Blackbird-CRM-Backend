namespace blackbird_crm.Models.ResponseModels.Clients
{
    public class ClientDetailsCommentsResponse
    {
        public int Id { get; set; } 
        public int ClientId { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
