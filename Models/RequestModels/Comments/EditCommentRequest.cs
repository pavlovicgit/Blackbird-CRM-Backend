namespace blackbird_crm.Models.RequestModels.Comments
{
    public class EditCommentRequest
    {
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public string CommentText { get; set; }
        
    }
}
