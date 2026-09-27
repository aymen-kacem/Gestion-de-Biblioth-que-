using CoworkingGes.Enum;
using System.Text.Json.Serialization;

namespace CoworkingGes.Models
{
    public class Abonnement
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;  // Mensuel, Annuel...
        public decimal Prix { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public Statut Statut { get; set; }
        public string? Notes { get; set; }


        public int UtilisateurId { get; set; }
        [JsonIgnore]
        public Utlisateur? Utilisateur { get; set; }

        public int EspaceId { get; set; }
        [JsonIgnore]
        public Espace? Espace { get; set; }
    }
}
