using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blackbird_crm.Models
{
    public class Project
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public string ProjectName { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; }
    }
}
