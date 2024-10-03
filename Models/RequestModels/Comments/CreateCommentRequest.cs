using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace blackbird_crm.Models.RequestModels.Comments
{
    public class CreateCommentRequest
    {
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public string CommentText { get; set; }
    }
}
