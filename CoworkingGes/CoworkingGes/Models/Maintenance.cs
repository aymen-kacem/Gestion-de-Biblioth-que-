using System.Text.Json.Serialization;

namespace CoworkingGes.Models
{
    public class Maintenance
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Statut { get; set; } 
        public DateTime Date { get; set; }

        
        public int EspaceId { get; set; }
        [JsonIgnore]
        public Espace? Espace { get; set; }
        public int TechnicienId { get; set; }
        [JsonIgnore]
        public Utlisateur? Technicien { get; set; }
    }
}
