namespace blackbird_crm.Models.RequestModels.Projects
{
    public class EditProjectRequest
    {
        public int ClientId { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
    }
}
