using CoworkingGes.Enum;
using System.Text.Json.Serialization;

namespace CoworkingGes.DTO
{
    public class AbonnementDTO
    {

 
        public int EspaceId { get; set; }
        [JsonIgnore]
        public int UtilisateurId { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public DateTime DateDebut { get; set; }
        [JsonIgnore]
        public DateTime DateFin { get; set; }
        public Statut Statut { get; set; }
        public string? Notes { get; set; }



    }
}
