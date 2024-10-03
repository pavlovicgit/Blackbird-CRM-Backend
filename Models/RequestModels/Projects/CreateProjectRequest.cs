namespace blackbird_crm.Models.RequestModels.Projects
{
    public class CreateProjectRequest
    {
        public int ClientId { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
    }
}
