using CoworkingGes.Enum;
using System.Text.Json.Serialization;

namespace CoworkingGes.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public Statut Statut { get; set; }
        public string? Notes { get; set; } // Pour les raisons de rejet ou commentaires
        public int UtilisateurId { get; set; }
        [JsonIgnore]
        public Utlisateur? Utilisateur { get; set; }
        public int EspaceId { get; set; }
        [JsonIgnore]
        public Espace? Espace { get; set; }
    }
}
