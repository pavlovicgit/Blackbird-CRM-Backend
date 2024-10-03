namespace blackbird_crm.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedDate { get; set; }
        public Client Client { get; set; }
        public Project Project { get; set; }
    }
}
